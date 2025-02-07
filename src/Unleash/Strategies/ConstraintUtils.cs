﻿using System.Collections.Generic;
using System.Linq;
using Unleash.Internal;
using Unleash.Strategies.Constraints;

namespace Unleash.Strategies
{
    public class ConstraintUtils
    {
        static Dictionary<string, IConstraintOperator> operators = new Dictionary<string, IConstraintOperator>()
        {
            { Operator.STR_CONTAINS, new StringConstraintOperator() },
            { Operator.STR_ENDS_WITH, new StringConstraintOperator() },
            { Operator.STR_STARTS_WITH, new StringConstraintOperator() },
            { Operator.NUM_EQ, new NumberConstraintOperator() },
            { Operator.NUM_GT, new NumberConstraintOperator() },
            { Operator.NUM_GTE, new NumberConstraintOperator() },
            { Operator.NUM_LT, new NumberConstraintOperator() },
            { Operator.NUM_LTE, new NumberConstraintOperator() },
            { Operator.DATE_AFTER, new DateConstraintOperator() },
            { Operator.DATE_BEFORE, new DateConstraintOperator() },
            { Operator.SEMVER_EQ, new SemverConstraintOperator() },
            { Operator.SEMVER_GT, new SemverConstraintOperator() },
            { Operator.SEMVER_LT, new SemverConstraintOperator() },
        };

        public static bool Validate(List<Constraint> constraints, UnleashContext context)
        {
            if (constraints?.Count > 0)
            {
                return constraints.TrueForAll(c => ValidateConstraint(c, context));
            }
            else
            {
                return true;
            }
        }

        private static bool ValidateConstraint(Constraint constraint, UnleashContext context)
        {
            var contextValue = context.GetByName(constraint.ContextName);
            if (constraint.Operator == null)
                return false;

            if (operators.ContainsKey(constraint.Operator))
                return operators[constraint.Operator].Evaluate(constraint, context);
            else
            {
                var isIn = contextValue != null && constraint.Values.Contains(contextValue.Trim());

                if (constraint.Operator == Operator.IN)
                    return isIn;
                if (constraint.Operator == Operator.NOT_IN)
                    return !isIn;
            }

            return false;
        }
    }
}