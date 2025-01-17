﻿namespace WhMgr.Services.Subscriptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Timers;
    using System.Threading.Tasks;

    using Microsoft.EntityFrameworkCore;
    //using Microsoft.Extensions.Logging;
    using InvasionCharacter = POGOProtos.Rpc.EnumWrapper.Types.InvasionCharacter;

    using WhMgr.Common;
    using WhMgr.Data.Contexts;
    using WhMgr.Extensions;
    using WhMgr.Services.Subscriptions.Models;

    public class SubscriptionManagerService : ISubscriptionManagerService
    {
        private readonly Microsoft.Extensions.Logging.ILogger<ISubscriptionManagerService> _logger;
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private List<Subscription> _subscriptions;
        private readonly Timer _timer;

        public IReadOnlyList<Subscription> Subscriptions => _subscriptions;

        public SubscriptionManagerService(
            Microsoft.Extensions.Logging.ILogger<ISubscriptionManagerService> logger,
            IDbContextFactory<AppDbContext> dbFactory)
        {
            _logger = logger;
            _dbFactory = dbFactory;

            _timer = new Timer(60 * 1000); // every minute TODO: Use config value
            _timer.Elapsed += async (_, _) => await ReloadSubscriptionsAsync();
            _timer.Start();

            Task.Run(async () => await ReloadSubscriptionsAsync(skipCheck: true));
        }

        #region Get Subscriptions

        public async Task<List<Subscription>> GetUserSubscriptionsAsync()
        {
            using var ctx = _dbFactory.CreateDbContext();
            _subscriptions = await ctx.Subscriptions.Where(s => s.Status != NotificationStatusType.None)
                                                    // Include Pokemon subscriptions
                                                    .Include(s => s.Pokemon)
                                                    // Include PvP subscriptions
                                                    .Include(s => s.PvP)
                                                    // Include Raid subscriptions
                                                    .Include(s => s.Raids)
                                                    // Include Quest subscriptions
                                                    .Include(s => s.Quests)
                                                    // Include Invasion subscriptions
                                                    .Include(s => s.Invasions)
                                                    // Include Lure subscriptions
                                                    .Include(s => s.Lures)
                                                    // Include Gym subscriptions
                                                    .Include(s => s.Gyms)
                                                    // Include Location subscriptions
                                                    .Include(s => s.Locations)
                                                    .ToListAsync();
            return _subscriptions;
        }

        public Subscription GetUserSubscriptions(ulong guildId, ulong userId)
        {
            return _subscriptions?.FirstOrDefault(sub => sub.GuildId == guildId && sub.UserId == userId);
        }

        #region Get By Type

        public List<Subscription> GetSubscriptionsByPokemonId(uint pokemonId)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.Pokemon) &&
                              sub.Pokemon != null &&
                              sub.Pokemon.Any(poke => poke.PokemonId.Contains(pokemonId))
                      )
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByPvpPokemonId(uint pokemonId)
        {
            return GetSubscriptionsByPvpPokemonId(new List<uint> { pokemonId });
        }

        public List<Subscription> GetSubscriptionsByPvpPokemonId(List<uint> pokemonId)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.PvP) &&
                              sub.PvP != null &&
                              sub.PvP.Any(pvp => pvp.PokemonId.Exists(pokeId => pokemonId.Contains(pokeId)))
                      )
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByRaidPokemonId(uint pokemonId)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.Raids) &&
                              sub.Raids != null &&
                              sub.Raids.Any(raid => raid.PokemonId.Contains(pokemonId))
                      )
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByQuest(string pokestopName, string reward)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.Quests) &&
                              sub.Quests != null &&
                              sub.Quests.Any(quest =>
                                  // Check if quest contains quest subscription reward keyword
                                  reward.Contains(quest.RewardKeyword)
                                  // Otherwise check if pokestop name is set and contains or matches quest
                                  // subscriptions desired pokestop name 
                                  || (
                                      (
                                          !string.IsNullOrEmpty(quest.PokestopName)
                                          && (pokestopName.Contains(quest.PokestopName)
                                      )
                                      || string.Equals(pokestopName, quest.PokestopName, StringComparison.OrdinalIgnoreCase)
                                     )
                              )
                      )
                ).ToList();
        }

        public List<Subscription> GetSubscriptionsByInvasion(string pokestopName, InvasionCharacter gruntType, List<uint> encounterRewards)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.Invasions) &&
                              sub.Invasions != null &&
                              sub.Invasions.Any(invasion =>
                            {
                                var subPokestopName = invasion.PokestopName;
                                var rewardMatches = invasion.RewardPokemonId.Intersects(encounterRewards);
                                var typeMatches = invasion.InvasionType.Contains(gruntType) && gruntType != InvasionCharacter.CharacterUnset;
                                var pokestopMatches = !string.IsNullOrWhiteSpace(subPokestopName) && !string.IsNullOrWhiteSpace(pokestopName) &&
                                (
                                    pokestopName.Contains(subPokestopName)
                                    || string.Equals(pokestopName, subPokestopName, StringComparison.OrdinalIgnoreCase)
                                );
                                return rewardMatches || typeMatches || pokestopMatches;
                            })
                        )
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByLure(string pokestopName, PokestopLureType lureType)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.Lures) &&
                              sub.Lures != null &&
                              sub.Lures.Any(lure =>
                                  lure.LureType?.Contains(lureType) ?? false
                                  || (
                                      !string.IsNullOrWhiteSpace(lure.PokestopName) &
                                      !string.IsNullOrWhiteSpace(pokestopName)
                                      && (
                                          pokestopName.Contains(lure.PokestopName)
                                          || string.Equals(pokestopName, lure.PokestopName, StringComparison.OrdinalIgnoreCase)
                                      )
                                  )
                              )
                      )
                .ToList();
        }

        public List<Subscription> GetSubscriptionsByGymName(string name)
        {
            return _subscriptions?
                .Where(sub => sub.IsEnabled(NotificationStatusType.Gyms) &&
                              sub.Gyms != null &&
                              sub.Gyms.Any(gym =>
                                  string.Equals(name, gym.Name, StringComparison.OrdinalIgnoreCase)
                                  || gym.Name.ToLower().Contains(name.ToLower())
                              )
                      )
                .ToList();
        }

        #endregion

        #endregion

        #region General Subscription Management

        public async Task<bool> CreateSubscriptionAsync<TEntity>(TEntity subscription) where TEntity : BaseSubscription
        {
            using var ctx = _dbFactory.CreateDbContext();
            ctx.Add(subscription);
            var result = await ctx.SaveChangesAsync();
            return result == 1;
        }

        public async Task<bool> UpdateSubscriptionAsync<TEntity>(int id, TEntity subscription) where TEntity : BaseSubscription
        {
            // TODO: UpdateSubscriptionAsync
            await Task.CompletedTask;
            return true;
        }

        public TEntity FindById<TEntity>(int id) where TEntity : BaseSubscription
        {
            using var ctx = _dbFactory.CreateDbContext();
            var result = ctx.Find<TEntity>(id);
            return result;
        }

        public async Task<TEntity> FindByIdAsync<TEntity>(int id) where TEntity : BaseSubscription
        {
            using var ctx = _dbFactory.CreateDbContext();
            var result = await ctx.FindAsync<TEntity>(id);
            return result;
        }

        public bool DeleteById<TEntity>(int id) where TEntity : BaseSubscription
        {
            using var ctx = _dbFactory.CreateDbContext();
            ctx.Remove(id);
            var result = ctx.SaveChanges();
            return result == 1;
        }

        public async Task<bool> DeleteByIdAsync<TEntity>(int id) where TEntity : BaseSubscription
        {
            using var ctx = _dbFactory.CreateDbContext();
            ctx.Remove(id);
            var result = await ctx.SaveChangesAsync();
            return result == 1;
        }

        #endregion

        public async Task SetSubscriptionStatusAsync(int subscriptionId, NotificationStatusType status)
        {
            using var ctx = _dbFactory.CreateDbContext();
            var subscription = await ctx.Subscriptions.FindAsync(subscriptionId);
            if (subscription == null)
            {
                // Failed to find subscription by id
                _logger.Error($"Failed to find subscription by id in order to set subscription status to: {status}");
                return;
            }
            subscription.Status = status;
            await ctx.SaveChangesAsync(true);

            await ReloadSubscriptionsAsync(skipCheck: true);
        }

        public async Task<bool> SaveAsync(Subscription subscription)
        {
            // Save subscription changes
            try
            {
                using (var ctx = _dbFactory.CreateDbContext())
                {
                    ctx.Update(subscription);
                    await ctx.SaveChangesAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to save subscription with id {subscription.Id} (UserId: {subscription.UserId}, GuildId: {subscription.GuildId}): {ex}");
                return false;
            }
        }

        /// <summary>
        /// Reload all user subscriptions
        /// </summary>
        public async Task ReloadSubscriptionsAsync(bool skipCheck = false, ushort reloadM = 5)
        {
            try
            {
                // Only reload based on last_changed timestamp in metadata table
                var lastModifiedTimestamp = GetLastModifiedTimestamp();
                var utcNow = DateTime.UtcNow.GetUnixTimestamp();
                var reloadMinutesMs = reloadM * 60 * 60;
                var delta = utcNow - lastModifiedTimestamp;

                // Check if last_modified was set within the last x minutes
                if (!skipCheck && delta > reloadMinutesMs)
                    return;

                // Updated, reload subscriptions
                var subs = await GetUserSubscriptionsAsync();
                if (subs == null)
                    return;

                _subscriptions = subs;

                _logger.Information("User subscriptions list reloaded.");
            }
            catch (Exception ex)
            {
                _logger.Error($"Failed to reload user subscriptions: {ex}");
            }
        }

        private ulong GetLastModifiedTimestamp()
        {
            using var ctx = _dbFactory.CreateDbContext();
            var lastModified = ctx.Metadata.Find("LAST_MODIFIED");
            var result = Convert.ToUInt64(Math.Round(double.TryParse(lastModified?.Value, out var value) ? value : 0));
            return result;
        }
    }
}