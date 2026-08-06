using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace FinancialTracker.StateMachines {
    public abstract class BaseStateMachine<T> : ObservableObject where T : Enum {
        public void DispatchEventNotify(T eventId) {
            DispatchEventImpl(eventId);
            OnPropertyChanged("stateId");
        }

        protected abstract void DispatchEventImpl(T eventId);
    }
}
