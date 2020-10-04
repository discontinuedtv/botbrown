import React from "react";
import { Sidebar } from "./Sidebar";
import { MainArea } from "./MainArea";
import { ConfigurationDialog } from './ConfigurationDialog';
import { useRecoilState } from "recoil";
import { configurationState } from './recoil/atoms';

export const App = () => {
    const [configuration, setConfiguration] = useRecoilState(configurationState);

    const areAllConfigurationsCompleted = () => {
        return configuration?.configurations?.every(x => x.isValid);
    }

    return <div id="app">
        {!areAllConfigurationsCompleted() && <ConfigurationDialog />}
        <Sidebar />
        <MainArea />
    </div>
}
