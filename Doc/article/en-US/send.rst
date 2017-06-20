====
Send
====

Send messages from any :code:`IHandler`
=======================================

:code:`Send()` is a Miruken.Mediator extension method that you can call on any IHandler. Send expects that there will be one handler who receives the message.

Here in a TeamController we create a context and add the TeamHandler to it's collection of handlers. Context implements IHandler so we can call Send on the context passing in the CreateTeam request. Mediator will look through the handlers in the context to find one that can handle the CreateTeam message.

.. literalinclude:: /example/mediator/withBaseClasses/teamController.cs

.. literalinclude:: /example/mediator/createTeam.cs

.. literalinclude:: /example/mediator/removeTeam.cs

.. literalinclude:: /example/mediator/teamResult.cs

Send does not require a response.  Notice that the RemoveTeam command does not return a response.

If the request does not implement :code:`IRequest<TResponse>` you can pass in the expected return type when you call send as demostrated below.

.. literalinclude:: /example/mediator/withPureClasses/teamController.cs

