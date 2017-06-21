====
Send
====

:code:`Send()` is a Miruken.Mediator extension method that you can call on any IHandler. Send expects that there will be one handler who receives the message.

Here is an example of an Asp.net Web Api Controller. Context implements IHandler.  We call Send on the context passing in the CreateTeam request. Mediator will look through the handlers in the context to find one that can handle the CreateTeam message and return a TeamResult.

.. literalinclude:: /example/team/teamController.cs

.. literalinclude:: /example/team/createTeam.cs

.. literalinclude:: /example/team/removeTeam.cs

.. literalinclude:: /example/team/teamResult.cs

Send does not require a response.  Notice that the RemoveTeam request does not implement :code:`IRequest<TResponse>` and the handler does not return a response.

If the request does not implement :code:`IRequest<TResponse>` and the handler returns a response, you can pass in the expected return type when you call send as demostrated below.

.. literalinclude:: /example/withPureClasses/teamController.cs

