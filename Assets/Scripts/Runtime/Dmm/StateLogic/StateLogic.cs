using System.Collections.Generic;
using Dmm.Log;

namespace Dmm.StateLogic
{
    public class StateLogic<TData> : IStateLogic<TData>
    {
        private readonly string _name;

        private readonly TData _data;

        public StateLogic(string name, TData data)
        {
            _name = name;
            _data = data;
        }

        private bool _running = false;

        public void Start()
        {
            _running = true;
        }

        public void Stop()
        {
            _running = false;
        }

        private readonly Dictionary<int, IState<TData>> _states = new Dictionary<int, IState<TData>>();

        public void AddState(IState<TData> state)
        {
            if (state == null)
            {
                return;
            }

            var code = state.GetStateCode();
            if (_states.ContainsKey(code))
            {
                _states[code] = state;
            }
            else
            {
                _states.Add(code, state);
            }
        }

        private StateResult _currentStateCode = null;

        public int GetCurrentStateCode()
        {
            if (_currentStateCode == null)
            {
                return -1;
            }

            return _currentStateCode.NextStateCode;
        }

        public void SwitchTo(int code)
        {
            if (_currentStateCode == null)
            {
                _currentStateCode = new StateResult();
                _currentStateCode.Result = StateResult.Null;
                _currentStateCode.NextStateCode = code;
                Reset();
                return;
            }

            if (_currentStateCode.NextStateCode == code)
            {
                return;
            }

            _currentStateCode.Result = StateResult.Null;
            _currentStateCode.NextStateCode = code;
            Reset();
        }

        public void Reset()
        {
            foreach (var state in _states.Values)
            {
                state.Reset();
            }
        }

        public StateResult Process(float time)
        {
            if (!_running)
            {
                return null;
            }

            if (_currentStateCode == null)
            {
                return null;
            }

            if (!_states.ContainsKey(_currentStateCode.NextStateCode))
            {
                _currentStateCode.Result = StateResult.StateNotFound;
                return _currentStateCode;
            }

            var state = _states[_currentStateCode.NextStateCode];
            if (state == null)
            {
                _currentStateCode.Result = StateResult.StateNotFound;
                return _currentStateCode;
            }

            if (!state.IsStarted())
            {
                MyLog.DebugWithFrame(_name, string.Format("Start state: {0}", state.GetStateName()));
                _currentStateCode.Result = StateResult.Started;
                state.Start();
                state.Initialize(_data, time);
            }

            if (state.IsAbort())
            {
                MyLog.DebugWithFrame(_name, string.Format("Abort state: {0}", state.GetStateName()));
                _currentStateCode.Result = StateResult.Aborted;
                return _currentStateCode;
            }

            var finish = state.Process(_data, time);

            if (state.IsAbort())
            {
                MyLog.DebugWithFrame(_name, string.Format("Abort state: {0}", state.GetStateName()));
                _currentStateCode.Result = StateResult.Aborted;
                return _currentStateCode;
            }

            if (finish)
            {
                MyLog.DebugWithFrame(_name, string.Format("Finish state: {0}", state.GetStateName()));
                _currentStateCode = state.Finish(_data, time);
                state.Reset();
            }

            return _currentStateCode;
        }

        public void OnPause(bool pause, float time)
        {
            if ((_currentStateCode == null) || !_states.ContainsKey(_currentStateCode.NextStateCode))
            {
                return;
            }

            var state = _states[_currentStateCode.NextStateCode];
            if (state != null)
            {
                state.OnPause(_data, pause, time);
            }
        }
    }
}