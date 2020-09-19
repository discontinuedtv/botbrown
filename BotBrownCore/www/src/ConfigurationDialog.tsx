import React, { useEffect, Suspense } from 'react';
import { Stepper, Step, StepLabel, StepButton, Button } from '@material-ui/core';
import { useRecoilState, useRecoilValue } from "recoil";
import { configurationState, currentConfigurationState } from "./recoil/atoms";
import { Configuration } from './Configuration';

export const ConfigurationDialog = () => {

    const [currentConfiguration, setCurrentConfiguration] = useRecoilState(currentConfigurationState);


    const handleStep = (step: number) => {

    }

    const isStepComplete = (step: number) => {
        return currentConfiguration?.steps[step].isCompleted;
    }

    const isOnFirstStep = () : boolean => {

        return true;
        //return currentConfiguration?.currentlyActiveStep === 0;
    }

    const isOnLastStep = () => {

        if (currentConfiguration === undefined) {
            return true;
        }

        //return currentConfiguration.currentlyActiveStep === currentConfiguration.steps.length - 1;
    }

    const handleBack = () => {

        if (currentConfiguration === undefined) {
            return;
        }

        //currentConfiguration.currentlyActiveStep = currentConfiguration.currentlyActiveStep - 1;
    }

    const handleNext = () => {
        //setCurrentConfiguration(c => c?.produce(x => x.moveToNextStep()));
    }

    return (
        
            <div className="modal">
                <Stepper>
                    <Step key="test1">
                        <StepButton
                            onClick={() => handleStep(1)}
                            completed={isStepComplete(1)}
                        >
                            Test 1
                    </StepButton>
                    </Step>
                    <Step key="test2">
                        <StepLabel>Test 2</StepLabel>
                    </Step>
                </Stepper>
                <div>
                    <Button disabled={isOnFirstStep()} onClick={handleBack}>
                        Back
              </Button>
                    <Button
                        disabled={isOnLastStep()}
                        variant="contained"
                        color="primary"
                        onClick={handleNext}
                    >
                        Next
              </Button>
                </div>
            </div>);
}
