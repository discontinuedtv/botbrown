import React, {useState} from "react";
import {Sidebar} from "./Sidebar";

export const App = () => {
    const [liked, setLiked] = useState(false);
    
    if (liked) {
        return <div>
            You bla liked this.
        </div>;
    }
    
    return <div>
        <Sidebar />
        <div id="main-area">
            Main content
        </div>
    </div>
}
