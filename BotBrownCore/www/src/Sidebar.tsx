import React from "react";
import { List, ListItem } from "@material-ui/core";
import { NavLink } from "react-router-dom";
import Logo from './logo.png';


export const Sidebar = () => {
    return (
        <div id='sidebar'>
            <div id="sidebar-logoarea">               
                <div><img src={Logo} /></div>
                <div>Bot Brown</div>
            </div>
            <List disablePadding dense id="sidebarmenulist">
                <NavLink to="/" exact activeClassName="sidebarmenuitem-active" className="sidebarmenuitem">
                    <ListItem button>Home</ListItem>
                </NavLink>
                <NavLink to="/commands/abc" activeClassName="sidebarmenuitem-active" className="sidebarmenuitem">
                    <ListItem button>Commands</ListItem>
                </NavLink>
            </List>
        </div>);
}