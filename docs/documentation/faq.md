# FAQ (Frequently asked questions)

* **Q** Why another {x} framework for load testing?

    **A** The main reasons are:
    - **To be technology agnostic** as much as possible (**no dependency on any protocol: HTTP, WebSockets, SSE etc**).
    - To be able to test .NET implementation of specific driver. During testing, it was identified many times that the performance could be slightly different because of the virtual machine(.NET, Java, PHP, Js, Erlang, different settings for GC) or just quality of drivers. For example there were cases that drivers written in C++ and invoked from NodeJs app worked faster than drivers written in C#/.NET. Therefore, it does make sense to load test your app using your concrete driver and runtime.    