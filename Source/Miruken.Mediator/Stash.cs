namespace Miruken.Mediator
{
    using System;
    using System.Collections.Generic;
    using Callback;

    public interface IStash
    {
        T    Get<T>() where T : class;
        void Put<T>(T data) where T : class;
        bool Drop<T>() where T : class;
    }

    public class Stash : Handler, IStash
    {
        private readonly bool _root;
        private readonly Dictionary<Type, object> _data;

        public Stash(bool root = false)
        {
            _root = root;
            _data = new Dictionary<Type, object>();
        }

        public T Get<T>() where T : class
        {
            object data;
            return _data.TryGetValue(typeof(T), out data)
                 ? (T)data : (_root ? null : Unhandled<T>());
        }

        public void Put<T>(T data) where T : class
        {
            _data[typeof(T)] = data;
        }

        public bool Drop<T>() where T : class
        {
            return _data.Remove(typeof(T));
        }
    }

    public static class StashExtensions
    {
        public static T TryGet<T>(this IStash stash)
          where T : class
        {
            try
            {
                return stash.Get<T>();
            }
            catch
            {
                return null;
            }
        }

        public static T GetOrPut<T>(this IStash stash, T put)
            where T : class
        {
            var data = stash.TryGet<T>();
            if (data == null)
            {
                data = put;
                stash.Put(data);
            }
            return data;
        }

        public static T GetOrPut<T>(this IStash stash, Func<T> put)
            where T : class
        {
            if (put == null)
                throw new ArgumentNullException(nameof(put));
            var data = stash.TryGet<T>();
            if (data == null)
            {
                data = put();
                stash.Put(data);
            }
            return data;
        }
    }
}
