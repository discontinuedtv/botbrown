import React from 'react';
import { atom, selector } from "recoil";
import { Configuration } from "../Configuration";
import { ConfigurationStep } from "../ConfigurationStep";

export class ConfigurationState {


    configurations: Array<Configuration>;

    constructor() {
        this.configurations = new Array<Configuration>();
        let testConfig1 = new Configuration();
        let step1 = new ConfigurationStep();
        step1.setContent(<div>Bla1</div>);
        testConfig1.addStep(step1);
        let step2 = new ConfigurationStep();
        step2.setContent(<div>Bla2</div>);
        testConfig1.addStep(step2);
        this.configurations.push(testConfig1);
    }
}

const asyncConfiguration = selector<ConfigurationsType | undefined>({
    key: "asyncConfiguration",
    get: async ({ get }) => {

        try {
            return api<ConfigurationsType | undefined>("http://localhost:12345/api/configurations", (err) => {
                console.log(err);
                return undefined;
            });
        }
        catch (e) {
            return undefined;
        }
    }
});

function api<T>(url: string, onError: (error: any) => T): Promise<T> {
    return fetch(url, { mode: 'no-cors' })
        .then(response => {
            if (!response.ok) {
                throw new Error(response.statusText)
            }
            return response.json() as Promise<T>
        })
        .catch(error => onError(error));
}

export type ConfigurationsType = {
    configurations: Array<ConfigurationType>
};

export type ConfigurationType = {
    name: string,
    steps: Array<StepType>
};

export type StepType = {
    name: string,
    isCompleted: boolean
};

export const configurationState = atom<ConfigurationsType | undefined>({
    key: "configurationState",
    default: asyncConfiguration
});

/*
export const configurationState = atom({
    key: "configurationState",
    default: new ConfigurationState()
});
*/

export const currentConfigurationState = selector<ConfigurationType | undefined>({
    key: 'currentConfigurationState',
    get: ({ get }) => {
        var config = get(configurationState);
        return config?.configurations.find(x => !x.steps.every(s => s.isCompleted));
    },
    set: ({ set }) => {
        console.log('setter called');
        console.log(set);
    }
});

export const configurationsCompletedState = atom({
    key: "configurationsComppletedState",
    default: new Set<number>()
});