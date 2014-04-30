using System;

namespace GoCartMonads
{
    public struct Maybe<T>
    {
        public Maybe(T value)
        {
            _value = value;
            _isNothing = _value == null;
        }

        private readonly bool _isNothing;
        private readonly T _value;

        private Maybe(bool isNothing, T value)
        {
            _isNothing = isNothing;
            _value = value;
        }

        public void Do(Action<T> f)
        {
            if(_isNothing) return;
            f(_value);
        }

        public void Do(Action<T> f, Action action)
        {
            if(_isNothing)
            {
                action();
                return;
            }
            f(_value);
        }

        public Maybe<TResult> With<TResult>(Func<T, TResult> f)
        {
            if(_isNothing) return Nothing<TResult>();
            return new Maybe<TResult>(f(_value));
        }

        public Maybe<TResult> With<TResult>(Func<T, Maybe<TResult>> f)
        {
            if(_isNothing) return Nothing<TResult>();
            return f(_value);
        }

        public static Maybe<TResult> Nothing<TResult>()
        {
            return new Maybe<TResult>(true, default(TResult));
        }

        public static Maybe<TInput> Just<TInput>(TInput input)
        {
            if(input == null) throw new ArgumentException("input cannot be null", "input");
            return new Maybe<TInput>(input);
        }

        public TResult CaseOf<TResult>(Func<T, TResult> f1, Func<TResult> f2)
        {
            if(_isNothing) return f2();
            return f1(_value);
        }

        public static Maybe<Func<TInput2, TResult>> FMap<TInput1, TInput2, TResult>(Func<TInput1, TInput2, TResult> f, Maybe<TInput1> o)
        {
            return o.With<Func<TInput2, TResult>>(t1 => t2 => f(t1, t2));
        }

        public static Maybe<TResult> FMap<TInput, TResult>(Func<TInput, TResult> f, Maybe<TInput> o)
        {
            return o.With(f);
        }

        public static Maybe<TResult> LiftM2<TInput1, TInput2, TResult>(Maybe<TInput1> o1, Maybe<TInput2> o2,
            Func<TInput1, TInput2, TResult> f)
        {
            return o1.With(t1 => o2.With(t2 => f(t1, t2)));
        }

        public static Maybe<TResult> LiftM3<TInput1, TInput2, TInput3, TResult>(Maybe<TInput1> o1, Maybe<TInput2> o2, Maybe<TInput3> o3,
                Func<TInput1, TInput2, TInput3, TResult> f)
        {
            var maybeFunc = LiftM2<TInput1, TInput2, Func<TInput3, TResult>>(o1, o2, (t1, t2) => t3 => f(t1, t2, t3));
            return maybeFunc.With(o3.With);
        }

        public static Maybe<TResult> LiftM4<TInput1, TInput2, TInput3, TInput4, TResult>(Maybe<TInput1> o1, Maybe<TInput2> o2, Maybe<TInput3> o3, Maybe<TInput4> o4,
                Func<TInput1, TInput2, TInput3, TInput4, TResult> f)
        {
            var maybeFunc = LiftM3<TInput1, TInput2, TInput3, Func<TInput4, TResult>>(o1, o2, o3, (t1, t2, t3) => t4 => f(t1, t2, t3, t4));
            return maybeFunc.With(o4.With);
        }
    }
}