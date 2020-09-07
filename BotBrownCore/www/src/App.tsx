import React, {useState} from "react";
import {Sidebar} from "./Sidebar";
import { MainArea } from "./MainArea";

export const App = () => {
    const [liked, setLiked] = useState(false);
    
    if (liked) {
        return <div>
            You bla liked this.
        </div>;
    }
    
    return <div id="app">
        <Sidebar />
        <MainArea />
    </div>
}
