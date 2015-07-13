using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public static class ReactiveObjectExtensions
    {
        private static void SetDeepValue<T, TObj>(TObj notUsed, Expression<Func<TObj, T>> propertyToSet, T valueToSet)
        {
            List<MemberInfo> members = new List<MemberInfo>();

            Expression exp = propertyToSet.Body;
            ConstantExpression ce = null;
            ParameterExpression tp = null;

            // There is a chain of getters in propertyToSet, with at the 
            // beginning a ConstantExpression. We put the MemberInfo of
            // these getters in members and the ConstantExpression in ce

            while (exp != null)
            {
                MemberExpression mi = exp as MemberExpression;

                if (mi != null)
                {
                    members.Add(mi.Member);
                    exp = mi.Expression;
                }
                else
                {
                    ce = exp as ConstantExpression;
                    tp = exp as ParameterExpression;

                    if (ce == null && tp == null)
                    {
                        // We support only a ConstantExpression at the base
                        // no function call like
                        // () => myfunc().A.B.C
                        //added support for parameters passed in 
                        throw new NotSupportedException();
                    }

                    break;
                }
            }

            if (members.Count == 0)
            {
                // We need at least a getter
                throw new NotSupportedException();
            }

            // Now we must walk the getters (excluding the last).
            // From the ConstantValue ce we take the base object
            object targetObject;

            if (ce != null)
            {
                targetObject = ce.Value;
            }
            else
            {
                targetObject = notUsed;
            }

            // We have to walk the getters from last (most inner) to second
            // (the first one is the one we have to use as a setter)
            for (int i = members.Count - 1; i >= 1; i--)
            {
                PropertyInfo pi = members[i] as PropertyInfo;

                if (pi != null)
                {
                    targetObject = pi.GetValue(targetObject);
                }
                else
                {
                    FieldInfo fi = (FieldInfo)members[i];
                    targetObject = fi.GetValue(targetObject);
                }
            }

            // The first one is the getter we treat as a setter
            {
                PropertyInfo pi = members[0] as PropertyInfo;

                if (pi != null)
                {
                    pi.SetValue(targetObject, valueToSet);
                }
                else
                {
                    FieldInfo fi = (FieldInfo)members[0];
                    fi.SetValue(targetObject, valueToSet);
                }
            }
        }

        public static IDisposable MapToMember<TMember, T>(this IObservable<TMember> observable, T vm, Expression<Func<T, TMember>> propertyFunc) where T : ReactiveObject
        {
            return observable.Subscribe(v =>
            {
                SetDeepValue(vm, propertyFunc, v);
            });
        }
    }
}
