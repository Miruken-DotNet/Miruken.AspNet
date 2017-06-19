====================
Request And Response
====================

A request is a message that you want mediator to handle.  Usually, a request has a response.  You can inherit from `IRequest<TResponse>` to delare what response the request expects.

Here the CreateTeam request inherits from IRequest and  expects to receive back a Team object:

.. literalinclude:: /example/teams/createTeam.cs

Requests do not have to inherit from IRequest.

Here RemoveTeam does not expect a response:

.. literalinclude:: /example/teams/removeTeam.cs




