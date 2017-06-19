=====
Intro
=====

The mediator pattern allows us to implement cross cutting concerns like error handling, logging, validation, and caching in a pipline that is portable between applications.  Asp.net has a pipeline, but it is tied to the web.  NServiceBus gives us a message handling pipeline, but that is tied specifically to NserviceBus. Using Miruken Mediator you have the power of a pipeline, in any C# application.  This means you can have a pipeline in web apps, web apis, windows services, console applications, WinForms, and Wpf applications.