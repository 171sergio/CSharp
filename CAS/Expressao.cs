using System;

namespace CAS
{
    public abstract class Expressao
    {
        public abstract override string ToString();
        public abstract Expressao Derivar(Simbolo x);
        public abstract Expressao Simplificar();
        public abstract Expressao Substituir(Simbolo s, Expressao e);

        public static Expressao operator +(Expressao a, Expressao b) => new Soma(a, b).Simplificar();
        public static Expressao operator -(Expressao a, Expressao b) => new Subtracao(a, b).Simplificar();
        public static Expressao operator *(Expressao a, Expressao b) => new Multiplicacao(a, b).Simplificar();
        public static Expressao operator /(Expressao a, Expressao b) => new Divisao(a, b).Simplificar();

        public static implicit operator Expressao(int v) => new Numero(v);
        public static implicit operator Expressao(string s) => new Simbolo(s);   
    }

    public class Numero : Expressao
    {
        public int valor;
        public Numero(int v) => this.valor = v;
        public override string ToString() => valor.ToString();
        public override Expressao Derivar(Simbolo x) => new Numero(0);
        public override Expressao Simplificar() => this;
        public override Expressao Substituir(Simbolo s, Expressao e) => this;
    }

    public class Simbolo : Expressao
    {
        string simbolo;
        public Simbolo(string s) => this.simbolo = s;
        public override string ToString() => simbolo;
        public override Expressao Derivar(Simbolo x) => 
            x.simbolo == simbolo 
                ? new Numero(1) 
                : new Numero(0);
        public override Expressao Simplificar() => this;
        public override Expressao Substituir(Simbolo s, Expressao e) =>
            s.simbolo == simbolo ? e : this;
    }

    public class Complexo : Expressao
    {
        public double Real { get; }
        public double Imaginario { get; }

        public Complexo(double real, double imaginario)
        {
            Real = real;
            Imaginario = imaginario;
        }

        public override string ToString() => $"{Real} + {Imaginario}i";
        public override Expressao Derivar(Simbolo x) => new Complexo(0, 0);
        public override Expressao Simplificar() => this;
        public override Expressao Substituir(Simbolo s, Expressao e) => this;
    }

    public class Soma : Expressao
    {
        Expressao a, b; // a + b
        public Soma(Expressao x, Expressao y)
        {
            this.a = x;
            this.b = y;
        }
        public override string ToString() => $"({a.ToString()} + {b.ToString()})";
        public override Expressao Derivar(Simbolo x) => 
            new Soma(a.Derivar(x), b.Derivar(x));
        public override Expressao Simplificar(){
           if(a is Numero && b is Numero)
           {
              return new Numero((a as Numero).valor + (b as Numero).valor);
           }
           if (a is Complexo && b is Complexo)
           {
               var ca = a as Complexo;
               var cb = b as Complexo;
               return new Complexo(ca.Real + cb.Real, ca.Imaginario + cb.Imaginario);
           }
            if (b is Numero && (b as Numero).valor == 0)
            {
                return a.Simplificar();
            }
            if (a is Numero && (a as Numero).valor == 0)
            {
                return b.Simplificar();
            }
           return this;
        }
        public override Expressao Substituir(Simbolo s, Expressao e) =>
            new Soma(a.Substituir(s, e), b.Substituir(s, e));
    }

    public class Subtracao : Expressao
    {
        Expressao a, b; // a - b
        public Subtracao(Expressao x, Expressao y)
        {
            this.a = x;
            this.b = y;
        }
        public override string ToString() => $"({a.ToString()} - {b.ToString()})";
        public override Expressao Derivar(Simbolo x) => 
            new Subtracao(a.Derivar(x), b.Derivar(x));
        public override Expressao Simplificar()
        {
            if(a is Numero && b is Numero)
            {
                return new Numero((a as Numero).valor - (b as Numero).valor);
            }
            if (a is Complexo && b is Complexo)
            {
                var ca = a as Complexo;
                var cb = b as Complexo;
                return new Complexo(ca.Real - cb.Real, ca.Imaginario - cb.Imaginario);
            }
            if (b is Numero && (b as Numero).valor == 0)
            {
                return a.Simplificar();
            }
            if (a is Numero && (a as Numero).valor == 0)
            {
                return new Subtracao(new Numero(0), b.Simplificar());
            }
            return this;
        }
        public override Expressao Substituir(Simbolo s, Expressao e) =>
            new Subtracao(a.Substituir(s, e), b.Substituir(s, e));
    }

    public class Multiplicacao : Expressao
    {
        Expressao a, b; // a * b
        public Multiplicacao(Expressao x, Expressao y)
        {
            this.a = x;
            this.b = y;
        }
        public override string ToString() => $"({a.ToString()} * {b.ToString()})";
        public override Expressao Derivar(Simbolo x) =>
            new Soma(
                new Multiplicacao(a.Derivar(x), b),
                new Multiplicacao(a, b.Derivar(x)));
        public override Expressao Simplificar()
        {
            if (a is Numero && b is Numero)
            {
                return new Numero((a as Numero).valor * (b as Numero).valor);
            }
            if (a is Complexo && b is Complexo)
            {
                var ca = a as Complexo;
                var cb = b as Complexo;
                return new Complexo(
                    ca.Real * cb.Real - ca.Imaginario * cb.Imaginario,
                    ca.Real * cb.Imaginario + ca.Imaginario * cb.Real);
            }
            if ((a is Numero && (a as Numero).valor == 0) || (b is Numero && (b as Numero).valor == 0))
            {
                return new Numero(0);
            }
            return this;
        }
        public override Expressao Substituir(Simbolo s, Expressao e) =>
            new Multiplicacao(a.Substituir(s, e), b.Substituir(s, e));
    }

    public class Divisao : Expressao
    {
        Expressao a, b; // a / b
        public Divisao(Expressao x, Expressao y)
        {
            this.a = x;
            this.b = y;
        }
        public override string ToString() => $"({a.ToString()} / {b.ToString()})";
        public override Expressao Derivar(Simbolo x) =>
            new Divisao(
                new Subtracao(
                    new Multiplicacao(a.Derivar(x), b), 
                    new Multiplicacao(a, b.Derivar(x))),
                new Multiplicacao(b, b));
        public override Expressao Simplificar()
        {
            if (a is Numero && b is Numero)
            {
                return new Numero((a as Numero).valor / (b as Numero).valor);
            }
            if (a is Complexo && b is Complexo)
            {
                var ca = a as Complexo;
                var cb = b as Complexo;
                var divisor = cb.Real * cb.Real + cb.Imaginario * cb.Imaginario;
                return new Complexo(
                    (ca.Real * cb.Real + ca.Imaginario * cb.Imaginario) / divisor,
                    (ca.Imaginario * cb.Real - ca.Real * cb.Imaginario) / divisor);
            }
            if (a is Numero && (a as Numero).valor == 0)
            {
                return new Numero(0);
            }
            if (b is Numero && (b as Numero).valor == 0)
            {
                throw new DivideByZeroException("Divisão por zero não é permitida.");
            }
            return this;
        }
        public override Expressao Substituir(Simbolo s, Expressao e) =>
            new Divisao(a.Substituir(s, e), b.Substituir(s, e));
    }
}