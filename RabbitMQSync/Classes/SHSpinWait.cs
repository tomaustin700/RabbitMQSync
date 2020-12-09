﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RabbitMQSync.Classes
{
    public static class SHSpinWait
    {


        public static bool SpinUntil(Func<bool> condition, TimeSpan timeout)
        {
            if (timeout.TotalMilliseconds <= 0)
                throw new InvalidOperationException("TIMEOUT HAS TO BE GREATER THAN 0");
            DateTime date = DateTime.Now;

            try
            {
                return SpinWait.SpinUntil(() =>
                {
                    if (DateTime.Now > date.AddMilliseconds(timeout.TotalMilliseconds))
                        throw new TimeoutException();
                    else
                        return condition.Invoke();

                }, timeout);
            }
            catch (TimeoutException)
            {
                return false;
            }
        }
    }
}
