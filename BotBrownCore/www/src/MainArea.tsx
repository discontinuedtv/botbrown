import React from "react";
import { Switch, Route } from 'react-router-dom';
import { HomePage } from './HomePage';
import { CommandsPage } from './CommandsPage';

export const MainArea = () => {
    return (
        <div id="main-area">
            <Switch>
                <Route exact path="/">
                    <HomePage />
                </Route>
                <Route path="/commands/:test">
                    <CommandsPage />
                </Route>
            </Switch>
        </div>);
}
