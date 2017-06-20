=======
Handler
=======

Create handlers for you requests
================================

To handle a request you need to create a method on a class with the :code:`[Mediates]` attribute.  The method takes the request object as the first parameter and returns the response object.

For example, here in TeamHandler.cs we have a method called CreateTeam that takes in a CreateTeam object and returns a TeamResult.  

.. literalinclude:: /example/mediator/withPureClasses/teamHandler.cs

Optionally the :code:`[Mediates]` method can take an IHandler as the second parameter.  We call this the composer.  

.. literalinclude:: /example/mediator/withComposer/teamHandler.cs

Use the composer within your method to send other messages or publish notifications.
