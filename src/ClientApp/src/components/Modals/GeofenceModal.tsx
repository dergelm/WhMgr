import React from 'react';
import {
    Box,
    Grid,
    Modal,
    Typography,
} from '@mui/material';

interface GeofenceModalProps {
    show: boolean;
    onClose: any;
    title: string;
    body: any;
}

export function GeofenceModal(props: GeofenceModalProps) {
    const style = {
        position: 'absolute' as 'absolute',
        top: '50%',
        left: '50%',
        transform: 'translate(-50%, -50%)',
        width: 400,
        bgcolor: 'background.paper',
        border: '2px solid #000',
        boxShadow: 24,
        p: 4,
    };

    return (
        <Modal
            open={props.show}
            onClose={props.onClose}
            aria-labelledby="modal-modal-title"
            aria-describedby="modal-modal-description"
        >
            <Box sx={style}>
                <Grid container spacing={2}>
                    <Grid item xs={12}>
                        <Typography>{props.title}</Typography>
                    </Grid>
                    <Grid item xs={12}>
                        {props.body}
                    </Grid>
                </Grid>
            </Box>
        </Modal>
    );
}