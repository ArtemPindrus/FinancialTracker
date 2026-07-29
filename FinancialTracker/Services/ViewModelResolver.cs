using FinancialTracker.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinancialTracker.Services {
    public class ViewModelResolver {
        private readonly IServiceProvider serviceProvider;

        public ViewModelResolver(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public T ResolveViewModel<T>() where T : ViewModelBase { 
            return serviceProvider.GetRequiredService<T>();
        }
    }
}
