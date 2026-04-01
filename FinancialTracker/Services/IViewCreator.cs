using Microsoft.Extensions.DependencyInjection;
using System;

namespace FinancialTracker.Services {
    public interface IViewCreator<T> where T : notnull {
        public T Create();
    }

    public class ViewCreator<T> : IViewCreator<T> where T : notnull {
        private readonly IServiceProvider serviceProvider;

        public ViewCreator(IServiceProvider serviceProvider) {
            this.serviceProvider = serviceProvider;
        }

        public T Create() => serviceProvider.GetRequiredService<T>();
    }
}
