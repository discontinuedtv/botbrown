import React from "react";
import {List, ListItem} from "@material-ui/core";


export const Sidebar = () => {
    return <List disablePadding dense>
        <ListItem button>Home</ListItem>
        <ListItem button>Test 1</ListItem>
        <ListItem button>Test 2</ListItem>
    </List>
}
