import React from 'react'
import { Link, useLocation } from 'react-router-dom';
import {
    Home as HomeIcon,
    Settings as SettingsIcon,
    ExitToApp as ExitToAppIcon,
    MiscellaneousServices as MiscellaneousServicesIcon,
    Notifications as NotificationsIcon,
    Storage as StorageIcon,
    FilterList as FilterListIcon,
    AccountTree as AccountTreeIcon,
    Navigation as NavigationIcon,
    Layers as LayersIcon,
    Subscriptions as SubscriptionsIcon,
    People as PeopleIcon,
} from '@mui/icons-material';
import { Container, Typography } from '@mui/material';
import { makeStyles } from '@mui/styles';
const { homepage } = require('../../../package.json');

const useStyles = makeStyles((theme: any) => ({
    container: {
        height: '100vh',
        //color: 'white',
        paddingTop: theme.spacing(10),
        backgroundColor: theme.palette.primary.main,
        position: 'sticky',
        top: 0,
        [theme.breakpoints.up('sm')]: {
            backgroundColor: 'white',
            color: '#555',
            border: '1px solid #ece7e7',
        },
    },
    item: {
        display: 'flex',
        alignItems: 'center',
        marginBottom: theme.spacing(4),
        [theme.breakpoints.up('sm')]: {
            marginBottom: theme.spacing(3),
            cursor: 'pointer',
        },
    },
    icon: {
        marginRight: theme.spacing(1),
        [theme.breakpoints.up('sm')]: {
            fontSize: '18px',
        },
    },
    text: {
        fontWeight: 500,
        [theme.breakpoints.down('sm')]: {
            display: 'none',
        },
    },
    link: {
        textDecoration: 'none',
        color: 'inherit',
    },
    active: {
        color: 'dodgerblue',
    },
}));

function Leftbar() {
    const classes = useStyles();
    const location = useLocation();
    const isActive = (page: string): any => {
        const { pathname } = location;
        const splitLocation = pathname.split('/');
        const isDashboard = page === splitLocation[2];
        const isPage = (splitLocation[2] && page.includes(splitLocation[2])) || (page === splitLocation[2] && !isDashboard);
        const result = splitLocation.length >= 2 && (isPage || isDashboard);
        if (result) {
            return classes.active;
        }
        return null;
    };
    return (
        <Container className={classes.container}>
            <Link to={homepage} className={classes.link}>
                <div className={isActive('')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <HomeIcon className={classes.icon} />
                    <Typography className={classes.text}>Dashboard</Typography>
                </div>
            </Link>
            <Link to={homepage + "configs"} className={classes.link}>
                <div className={isActive('configs')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <MiscellaneousServicesIcon className={classes.icon} />
                    <Typography className={classes.text}>Configs</Typography>
                </div>
            </Link>
            <Link to={homepage + "discords"} className={classes.link}>
                <div className={isActive('discords')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <StorageIcon className={classes.icon} />
                    <Typography className={classes.text}>Discords</Typography>
                </div>
            </Link>
            <Link to={homepage + "alarms"} className={classes.link}>
                <div className={isActive('alarms')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <NotificationsIcon className={classes.icon} />
                    <Typography className={classes.text}>Alarms</Typography>
                </div>
            </Link>
            <Link to={homepage + "filters"} className={classes.link}>
                <div className={isActive('filters')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <FilterListIcon className={classes.icon} />
                    <Typography className={classes.text}>Filters</Typography>
                </div>
            </Link>
            <Link to={homepage + "embeds"} className={classes.link}>
                <div className={isActive('embeds')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <AccountTreeIcon className={classes.icon} />
                    <Typography className={classes.text}>Embeds</Typography>
                </div>
            </Link>
            <Link to={homepage + "geofences"} className={classes.link}>
                <div className={isActive('geofences')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <NavigationIcon className={classes.icon} />
                    <Typography className={classes.text}>Geofences</Typography>
                </div>
            </Link>
            <Link to={homepage + "roles"} className={classes.link}>
                <div className={isActive('roles')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <LayersIcon className={classes.icon} />
                    <Typography className={classes.text}>Discord Roles</Typography>
                </div>
            </Link>
            <Link to={homepage + "subscriptions"} className={classes.link}>
                <div className={isActive('subscriptions')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <SubscriptionsIcon className={classes.icon} />
                    <Typography className={classes.text}>Subscriptions</Typography>
                </div>
            </Link>
            <Link to={homepage + "users"} className={classes.link}>
                <div className={isActive('users')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <PeopleIcon className={classes.icon} />
                    <Typography className={classes.text}>Users</Typography>
                </div>
            </Link>
            <Link to={homepage + "settings"} className={classes.link}>
                <div className={isActive('settings')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <SettingsIcon className={classes.icon} />
                    <Typography className={classes.text}>Settings</Typography>
                </div>
            </Link>
            <Link to={homepage + "logout"} className={classes.link}>
                <div className={isActive('logout')} style={{display: 'flex', alignItems: 'center', marginBottom: '20px'}}>
                    <ExitToAppIcon className={classes.icon} />
                    <Typography className={classes.text}>Logout</Typography>
                </div>
            </Link>
        </Container>
    );
}

export default Leftbar;