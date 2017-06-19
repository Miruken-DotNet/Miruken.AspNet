=======
Handler
=======

Create handlers for you requests.

To handle a request you simply need to create a method on a handler with the :code:`[Mediates]` attribute.  The method takes the request object as the first parameter and returns the response object.

For example, here in TeamMediator.cs we have a method called CreateTeam that takes in a CreateTeam object and returns a TeamResult.  

.. literalinclude:: /example/mediator/withBaseClasses/teamMediator.cs

Optionally the :code:`[Mediates]` method can take an IHandler as the second parameter.