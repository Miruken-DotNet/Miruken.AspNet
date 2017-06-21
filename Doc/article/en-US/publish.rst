=======
Publish
=======

:code:`Publish()` is a Miruken.Mediator extension method that you can call on any IHandler. 

Publish is used for notification.  It expects that there will be zero or more handlers that receive the message. Published messages do not receive a response. 

In this example we are executing :code:`Publish()` off of the IHandler composer that is passed in to the :code:`[Mediates]` method.  Mediator will find and execute all handlers that can handle TeamCreated.

.. literalinclude:: /example/handler/withComposer/teamHandler.cs
