using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Planning.Logic
{
    /**
     * A literal is an atomic expression (i.e. one which cannot be decomposed into
     * smaller expressions) or the negation of such an atomic expression.
     * 
     * @author Stephen G. Ware
     * @ported Edward Thomas Garcia
     */
    public interface Literal : Expression
    {
        new Literal Substitute(Substitution substitution);
        new Literal Negate();

        // @Override public default boolean isTestable() { return true; }
        // @Override public default boolean isTrue(State state) { return state.isTrue(this); }
        // @Override public default boolean isImposable() { return true; }
        // @Override public default void impose(MutableState state) { state.impose(this); }
        // @Override public default Expression toCNF() { return NormalForms.toCNF(this); }
        // @Override public default Expression toDNF() { return NormalForms.toDNF(this); }
        // @Override public default Expression simplify() { return this; }
    }
}
