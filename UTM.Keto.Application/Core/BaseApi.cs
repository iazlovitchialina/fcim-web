using System;
using UTM.Keto.Infrastructure;

namespace UTM.Keto.Application.Core
{
    public abstract class BaseApi : IDisposable
    {
        protected readonly ApplicationDbContext Context;

        protected BaseApi()
        {
            Context = new ApplicationDbContext();
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
