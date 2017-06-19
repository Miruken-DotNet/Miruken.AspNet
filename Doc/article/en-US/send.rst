====
Send
====

:code:`Send()` is a Miruken.Mediator extension methods that you can call on any IHandler. 

Send expects that there will be one handler who receives the message. The handler can return a response, but it does not have to.  

Here in a TeamController we create a context and add the TeamMediator to it's collection of handlers. Context implements IHandler so we can call Send on the context passing in the CreateTeam request.

.. literalinclude:: /example/mediator/withBaseClasses/teamController.cs

Mediator will look through the handlers in the context to find one that can Mediate the CreateTeam message.
