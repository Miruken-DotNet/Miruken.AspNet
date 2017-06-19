====================
Request And Response
====================

Create request and response objects
====================================

If the request inherits from :code:`IRequest<TResponse>` mediator will know the type of object it is returning.

.. literalinclude:: /example/mediator/withBaseClasses/createTeam.cs

.. literalinclude:: /example/mediator/teamResult.cs

Mediator can still handle your request even if you don't inherit from :code:`IRequest<TResponse>` you will just have to specify the return type when sending the message.

.. literalinclude:: /example/mediator/pureClasses/createTeam.cs

Responses are optional.  You can send requests that have no response.
