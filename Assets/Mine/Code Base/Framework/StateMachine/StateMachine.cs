using System;
using UniRx;

namespace Mine.CodeBase.Framework.StateMachine
{
    [Serializable]
    public class StateMachine<T>
        where T : Enum
    {
        #region Properties

        public T CurrentState => _currentState;
        public bool IsPlay {  get; set; }

        public IObservable<T> OnBegin => _beginSubject.Share();
        public IObservable<T> OnEnd => _endSubject.Share();
        public IObservable<T> OnUpdateStream => Observable.EveryUpdate().Select(_ => _currentState).Publish().RefCount();
        public IObservable<T> OnFixedUpdateStream => Observable.EveryFixedUpdate().Select(_ => _currentState).Publish().RefCount();
        public IObservable<T> OnLateUpdateStream => Observable.EveryLateUpdate().Select(_ => _currentState).Publish().RefCount();

        #endregion


        #region Fields

        private T _currentState;

        private Subject<T> _beginSubject = new();
        private Subject<T> _endSubject = new();

        #endregion


        #region Public Methods

        /// <summary>
        /// State machine start
        /// </summary>
        public void StartFSM(T initState)
        {
            IsPlay = true;
            _currentState = initState;
            _beginSubject.OnNext(initState);
        }

        /// <summary>
        /// Terminate the state machine
        /// </summary>
        public void FinishFSM(T state)
        {
            _endSubject.OnNext(_currentState);
            IsPlay = false;
        }

        public void Transition(T state)
        {
            _endSubject.OnNext(_currentState);

            _currentState = state;
            _beginSubject.OnNext(state);
        }

        public IObservable<T> OnBeginState(T state) => OnBegin.Where(x => x.Equals(state));
        public IObservable<T> OnEndState(T state) => OnEnd.Where(x => x.Equals(state));
        public IObservable<T> OnUpdateState(T state) => OnUpdateStream.Where(x => x.Equals(state));
        public IObservable<T> OnFixedUpdateState(T state) => OnFixedUpdateStream.Where(x => x.Equals(state));
        public IObservable<T> OnLateUpdateState(T state) => OnLateUpdateStream.Where(x => x.Equals(state));

        #endregion
    }
}
