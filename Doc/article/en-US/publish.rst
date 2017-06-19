=======
Publish
=======

:code:`Publish()` is a Miruken.Mediator extension methods that you can call on any IHandler. 

Publish is used for notification.  It expects that there will be zero or more handlers that receive the message. Published messages cannot receive a response. 