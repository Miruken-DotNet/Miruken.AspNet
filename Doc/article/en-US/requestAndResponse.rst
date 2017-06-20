====================
Request And Response
====================

Create request and response objects
====================================

By inheriting from :code:`IRequest<TResponse>` mediator will know the type of object being returned.

.. literalinclude:: /example/mediator/createTeam.cs

.. literalinclude:: /example/mediator/teamResult.cs

:code:`IRequest<TResponse>` is optional.  Mediator will still handle your request without it, but you will have to specify the return type when sending the message.

.. literalinclude:: /example/mediator/withPureClasses/createTeam.cs

Responses are optional.  You can send requests that have no response.
