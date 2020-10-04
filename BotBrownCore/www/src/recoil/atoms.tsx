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
            var result = await httpGet<ConfigurationsType>("http://localhost:43210/api/configurationstatus");
            if (result.parsedBody === undefined) {
                throw new Error('HILFE!');
            }

            return result.parsedBody;
        }
        catch (e) {
            return undefined;
        }
    }
});

interface HttpResponse<T> extends Response {
    parsedBody?: T;
}

export async function httpGet<T>(
    path: string,
    args: RequestInit = {
        method: "get",
        mode: "cors",
        headers: new Headers({ 'content-type': 'application/json' })
    }
): Promise<HttpResponse<T>> {
    return await http<T>(new Request(path, args));
};

export async function post<T>(
    path: string,
    body: any,
    args: RequestInit = { method: "post", body: JSON.stringify(body) }
): Promise<HttpResponse<T>> {
    return await http<T>(new Request(path, args));
};

export async function put<T>(
    path: string,
    body: any,
    args: RequestInit = { method: "put", body: JSON.stringify(body) }
): Promise<HttpResponse<T>> {
    return await http<T>(new Request(path, args));
};

export async function http<T>(
    request: RequestInfo
): Promise<HttpResponse<T>> {
    const response: HttpResponse<T> = await fetch(
        request
    );

    response.parsedBody = await response.json();
    return response;
}

export type ConfigurationsType = {
    configurations: Array<ConfigurationType>
};

export type ConfigurationType = {
    name: string,
    isValid: boolean,
    values: Array<ValueType>
};

export type StepType = {
    
};

export type ValueType = {
    typename: string,
    currentValue: string,
    defaultValue: string,
    isValid: boolean
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

export const currentConfigurationState = selector<Array<ConfigurationType>>({
    key: 'currentConfigurationState',
    get: ({ get }) => {
        var config = get(configurationState);
        if (config === undefined) {
            return new Array<ConfigurationType>();
        }

        return config.configurations.filter(x => !x.isValid);
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