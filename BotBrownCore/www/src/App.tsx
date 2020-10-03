import React, { useEffect, Suspense } from "react";
import { Sidebar } from "./Sidebar";
import { MainArea } from "./MainArea";
import { Configuration } from "./Configuration";
import { ConfigurationStep } from "./ConfigurationStep";
import { ConfigurationDialog } from './ConfigurationDialog';
import { useRecoilState } from "recoil";
import { configurationState } from './recoil/atoms';

export const App = () => {
    const [configuration, setConfiguration] = useRecoilState(configurationState);

    const areAllConfigurationsCompleted = () => {
        return configuration?.configurations?.every(x => x.steps.every(s => s.isCompleted));
    }

    return <div id="app">
        {areAllConfigurationsCompleted() && <ConfigurationDialog />}
        <Sidebar />
        <MainArea />
    </div>
}
