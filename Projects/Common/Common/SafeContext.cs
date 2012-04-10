using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common
{
    public static class SafeContext
    {
        public static void Execute(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public static T Execute<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return default(T);
            }
        }
    }
}