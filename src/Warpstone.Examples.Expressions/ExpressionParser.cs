﻿using static Warpstone.Parsers;
using static Warpstone.ExpressionParser;

namespace Warpstone.Examples.Expressions
{
    public static class ExpressionParser
    {
        private static readonly Parser<Expression> Num
            = Regex("[0-9]+").Transform(x => new NumExpression(int.Parse(x)) as Expression).Trim();

        private static readonly Parser<Expression> Parenthesized
            = Char('(')
            .Then(Lazy(() => Exp).Trim())
            .ThenSkip(Char(')'))
            .Trim();

        private static readonly Parser<Expression> Atom
            = Or(Parenthesized, Num).Trim();

        private static readonly Parser<Expression> Exp
            = BinaryExpression(Atom, new[]
            {
                LeftToRight<Expression, char>(Operator('*'), (l, r) => new MulExpression(l, r)),
                LeftToRight<Expression, char>(Operator('+'), (l, r) => new AddExpression(l, r))
            });

        private static Parser<T> Trim<T>(this Parser<T> parser)
            => OptionalWhitespaces.Then(parser).ThenSkip(OptionalWhitespaces);

        private static Parser<char> Operator(char c)
            => Char(c).Trim();

        public static Expression Parse(string input)
            => Exp.ThenEnd().Parse(input);
    }
}
