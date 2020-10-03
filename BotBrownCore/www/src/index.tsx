import React, { Suspense } from "react";
import { render } from "react-dom";
import { App } from "./App";
import { HashRouter } from "react-router-dom";
import { RecoilRoot } from "recoil";

render(
    <RecoilRoot>
        <HashRouter>
            <Suspense fallback={<div>Loading</div>}>
                <App />
            </Suspense>
        </HashRouter>
    </RecoilRoot>,
    document.getElementById('root'),
)
