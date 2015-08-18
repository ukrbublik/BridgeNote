using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace BridgeProject
{
    public static class BridgeGameScoring
    {
        public static Pairs RelativePair = Pairs.NS;


        // Простые действия для IntData
        static public void IntData_Equal(IntData data, int i)
        {
            data.Value = i;
        }

        static public void IntData_Sum(IntData data, int a, int b)
        {
            data.Value = a + b;
        }

        // Подсчет total score роббера
        static public void SetRobberScore(Robber rob, RobberScore robTScore)
        {
            rob.SetTotalScore(robTScore);
        }

        // Корректирование рез-та при изменении контракта
        static public void CorrectResultAccordingToContract(Result out_res, Contract contract, int method)
        {
            out_res.CorrectWhenContractChanged(contract, (Result.CorrectMethods)method);
        }

        // Если контракт "ВСЕ ПАС", пара - пусто
        static public void NoPairIfNoContract(PairSwitcher out_pair, Contract contract)
        {
            if (contract != null && contract.NoContract)
            {
                out_pair.Pair = Pairs.NotDefinedYet;
            }
        }

        //Определение сдающего
        enum QuartersFollow { N = 0, E, S, W };
        static QuartersFollow Q_ToFollow(Quarters q)
        {
            switch (q)
            {
                case Quarters.N:
                    return QuartersFollow.N;
                case Quarters.E:
                    return QuartersFollow.E;
                case Quarters.S:
                    return QuartersFollow.S;
                case Quarters.W:
                    return QuartersFollow.W;
                default:
                    return QuartersFollow.N;
            }
        }
        static Quarters Q_FromFollow(QuartersFollow qf)
        {
            switch (qf)
            {
                case QuartersFollow.N:
                    return Quarters.N;
                case QuartersFollow.E:
                    return Quarters.E;
                case QuartersFollow.S:
                    return Quarters.S;
                case QuartersFollow.W:
                    return Quarters.W;
                default:
                    return Quarters.NotDefinedYet;
            }
        }

        // Определение зоны/сдающего для роббера
        static public void DefineRobberDealer(QuarterSwitcher out_dealer, int fulldealNo, int robNo, int rdealNo, Quarters firstdealer)
        {
            QuartersFollow first_ = Q_ToFollow(firstdealer);
            QuartersFollow now_ = (QuartersFollow)(((int)first_ + fulldealNo) % 4);
            out_dealer.Quarter = Q_FromFollow(now_);
        }
        static public void DefineRobberZone(ZoneSwitcher out_zone, Robber rob, int robdealNo)
        {
            out_zone.Zone = rob.WhatZone(robdealNo);
        }

        // Определение зоны/сдающего для спорт.
        static public void DefineSportZone(ZoneSwitcher out_zone, int fulldealNo, int matchNo, int mdealNo, bool bZoneSwims)
        {
            int first_ = (int)Zones.None - 1;
            int now_;
            if (!bZoneSwims)
                now_ = (first_ + mdealNo) % 4;
            else
                now_ = (first_ + mdealNo + (int)(mdealNo / 4)) % 4;
            Zones z = (Zones)(now_ + 1);
            if (matchNo % 2 == 1) //каждый второй матч инвертировать зону
            {
                if (z == Zones.NS)
                    z = Zones.EW;
                else if (z == Zones.EW)
                    z = Zones.NS;
            }
            /*** было ***
            if (!bZoneSwims)
                now_ = (first_ + fulldealNo) % 4;
            else
                now_ = (first_ + fulldealNo + (int)(fulldealNo / 4)) % 4;
            ************/
            out_zone.Zone = z;
        }
        static public void DefineSportDealer(QuarterSwitcher out_dealer, int fulldealNo, int matchNo, int mdealNo, Quarters firstdealer, bool bZoneSwims)
        {
            QuartersFollow first_ = Q_ToFollow(firstdealer);
            QuartersFollow now_;
            if (bZoneSwims)
                now_ = (QuartersFollow) (((int)first_ + mdealNo) % 4);
            else
                now_ = (QuartersFollow) (((int)first_ + mdealNo + (int)(mdealNo / 4) + matchNo) % 4);

            /*** было ***
            if (bZoneSwims)
                now_ = (QuartersFollow)(((int)first_ + fulldealNo) % 4);
            else
                now_ = (QuartersFollow)(((int)first_ + fulldealNo + (int)(fulldealNo / 4)) % 4);
            *************/

            out_dealer.Quarter = Q_FromFollow(now_);
        }


        // Разница очков
        static public void ScoreDiff(SimpleScore out_diff, SimpleScore score1, SimpleScore score2)
        {
            if (out_diff == null)
                return;

            if (score1 == null || !score1.IsDefined() || score2 == null || !score2.IsDefined())
            {
                out_diff.Born = false;
            }
            else
            {
                int iNS = score1.Score.NS - score2.Score.NS;
                int iEW = score1.Score.EW - score2.Score.EW;

                if (iNS >= iEW)
                {
                    iNS -= iEW;
                    iEW = 0;
                }
                else
                {
                    iEW -= iNS;
                    iNS = 0;
                }
                out_diff.SetScore(iNS, iEW);
            }
        }

        // Сумма очков
        static public void ScoreSumm(SimpleScore out_sum, SimpleScore score1, SimpleScore score2)
        {
            if (out_sum == null)
                return;

            if (score1 == null || !score1.IsDefined() || score2 == null || !score2.IsDefined())
            {
                out_sum.Born = false;
            }
            else
            {
                int iNS = score1.Score.NS + score2.Score.NS;
                int iEW = score1.Score.EW + score2.Score.EW;

                if (iNS >= iEW)
                {
                    iNS -= iEW;
                    iEW = 0;
                }
                else
                {
                    iEW -= iNS;
                    iNS = 0;
                }
                out_sum.SetScore(iNS, iEW);
            }
        }

        //Онеры (для роббера)
        static public void GetOners(OnersSwitcher oners, Contract contract, CardsDistribution cd)
        {
            // Определить масть, запомнить старую масть
            CardTrump old_trump = oners.trump;
            if (contract == null || !contract.IsDefined() || contract.NoContract)
            {
                oners.trump = CardTrump.NotYetDefined;
            }
            else
            {
                oners.trump = contract.Trump;
            }

            // Если масть неизвестна, то отменить онеры
            if (oners.trump == CardTrump.NotYetDefined)
            {
                oners.Choise = 0; //-
            }

            // Взять из распределения карт
            if (cd != null && cd.IsDefined())
            {
                oners.Choise = CountOners(cd, oners.trump);
            }
            //Использовать введенное пользователем значение
            else
            {
                if (old_trump != oners.trump) //масть изменилась, отменить онеры
                    oners.Choise = 0; //-
            }
        }

        static public int CountOners(CardsDistribution cd, CardTrump trump)
        //возврат: 0 "-", 1 "NS 4o", 2 "NS 5o", 3 "NS 4A", 4 "EW 4o", 5 "EW 5o", 6 "EW 4A"
        {
            if (trump == CardTrump.NotYetDefined)
            {
                return 0; //-
            }
            // Посчитать, кому приндледат 4 туза
            else if (trump == CardTrump.NT)
            {
                int countN = 0, countS = 0, countE = 0, countW = 0;
                Quarters q;
                for (CardSuit cs = (CardSuit)0; cs < (CardSuit)4; cs++)
                {
                    q = cd.Get(CardValue.Ace, cs);
                    switch (q)
                    {
                        case Quarters.N:
                            countN++;
                            break;
                        case Quarters.S:
                            countS++;
                            break;
                        case Quarters.E:
                            countE++;
                            break;
                        case Quarters.W:
                            countW++;
                            break;
                    }
                }
                if (countN == 4 || countS == 4)
                    return 3; //NS 4A
                else if (countE == 4 || countW == 4)
                    return 6; //EW 4A
                else
                    return 0; //-
            }
            // Посчитать, кому принадлежат козырные онеры
            else
            {
                int countN = 0, countS = 0, countE = 0, countW = 0;
                Quarters q;
                for (CardValue cv = CardValue.Ten; cv <= CardValue.Ace; cv++)
                {
                    q = cd.Get(cv, SmallHelper.TrumpToSuit(trump));
                    switch (q)
                    {
                        case Quarters.N:
                            countN++;
                            break;
                        case Quarters.S:
                            countS++;
                            break;
                        case Quarters.E:
                            countE++;
                            break;
                        case Quarters.W:
                            countW++;
                            break;
                    }
                }
                if (countN == 5 || countS == 5)
                    return 2; //NS 5o
                else if (countE == 5 || countW == 5)
                    return 5; //EW 5o
                else if (countN == 4 || countS == 4)
                    return 1; //NS 4o
                else if (countE == 4 || countW == 4)
                    return 4; //EW 4o
                else
                    return 0; //-
            }
        }

         /*static public void GetCompensation(SimpleScore out_comp, IntData fig, ZoneSwitcher zone, PairSwitcher pair)
        {
            if (out_comp == null)
                return;

            if (fig == null || !fig.IsDefined() || zone == null || !zone.IsDefined() || pair == null || !pair.IsDefined())
            {
                out_comp.Born = false;
            }
            else
            {
                int NS_figs, EW_figs;
                if (pair.Pair == Pairs.NS)
                {
                    NS_figs = fig.Value;
                    EW_figs = 40 - fig.Value;
                }
                else
                {
                    EW_figs = fig.Value;
                    NS_figs = 40 - fig.Value;
                }
                out_comp.SetScore(Compens(NS_figs, ZoneSwitcher.IsPairInZone(Pairs.NS, zone.Zone)), Compens(EW_figs, ZoneSwitcher.IsPairInZone(Pairs.EW, zone.Zone)));
            }
        }*/

        static public int CountFigurs(CardsDistribution cd, Pairs pair)
        {
            int count = 0;
            Quarters q;
            for (CardValue cv = CardValue.Jack; cv <= CardValue.Ace; cv++)
            {
                for (CardSuit cs = (CardSuit)0; cs < (CardSuit)4; cs++)
                {
                    q = cd.Get(cv, cs);
                    if (pair == Pairs.NS && (q == Quarters.N || q == Quarters.S) || pair == Pairs.EW && (q == Quarters.E || q == Quarters.W))
                        count += (cv - CardValue.Jack + 1);
                }
            }
            return count;
        }

        // Сколько очков за фигуры?
        static public void FiguresPoints(IntData out_figs, CardsDistribution cd, bool useReletivePair, PairSwitcher pair)
        {
            if (out_figs == null)
                return;

            if (!useReletivePair) //использовать pair
            {
                // Если пара неизвестна, то не принимать введенные фигуры
                if (pair == null || !pair.IsDefined())
                {
                    out_figs.Born = false;
                }
                else
                {
                    // Если известно распределение карт, то заблокировать кол-во фигур на полученное из распределения
                    if (cd != null && cd.IsDefined())
                    {
                        out_figs.Value = CountFigurs(cd, pair.Pair);
                    }
                    else
                    {

                    }
                }
            }
            else //использовать RelativePair !!!!!
            {
                if (cd != null && cd.IsDefined())
                {
                    out_figs.Value = CountFigurs(cd, RelativePair);
                }
                else
                {

                }
            }
        }

        static public int CountFits(CardsDistribution cd, Pairs pair, bool TenCardsIsTwoFits)
        {
            int count = 0; //кол-во фитов

            int[] sum = new int[4] { 0, 0, 0, 0 }; //сумма карт каждой масти
            Quarters q;
            for (CardSuit cs = (CardSuit)0; cs < (CardSuit)4; cs++)
            {
                for (CardValue cv = CardValue.Two; cv <= CardValue.Ace; cv++)
                {
                    q = cd.Get(cv, cs);

                    if (pair == Pairs.NS && (q == Quarters.N || q == Quarters.S) || pair == Pairs.EW && (q == Quarters.E || q == Quarters.W))
                        sum[(int)cs]++;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (sum[i] >= 10 && TenCardsIsTwoFits)
                    count += 2;
                else if (sum[i] >= 8)
                    count++;
            }

            return count;
        }

        static public Pairs Whose1FitStronger(CardsDistribution cd) //чей 1 фит сильнее?
        {
            int[] sumNS = new int[4] { 0, 0, 0, 0 }; //сумма карт каждой масти (для NS)
            int[] sumEW = new int[4] { 0, 0, 0, 0 }; //сумма карт каждой масти (для EW)

            Quarters q;
            for (CardSuit cs = (CardSuit)0; cs < (CardSuit)4; cs++)
            {
                for (CardValue cv = CardValue.Two; cv <= CardValue.Ace; cv++)
                {
                    q = cd.Get(cv, cs);

                    if (q == Quarters.N || q == Quarters.S)
                        sumNS[(int)cs]++;
                    else if (q == Quarters.E || q == Quarters.W)
                        sumEW[(int)cs]++;
                }
            }

            CardSuit suitNS = (CardSuit)4, suitEW = (CardSuit)4;
            for (int i = 0; i < 4; i++)
            {
                if (sumNS[i] >= 8)
                    suitNS = (CardSuit)i;
                if (sumEW[i] >= 8)
                    suitEW = (CardSuit)i;
            }

            if (SmallHelper.CompareSuits(suitNS, suitEW) == 1)
                return Pairs.NS;
            else if (SmallHelper.CompareSuits(suitNS, suitEW) == -1)
                return Pairs.EW;
            else
                return Pairs.NotDefinedYet;
        }

        static public Pairs WhoHasSpadesFit(CardsDistribution cd) //у кого пиковый фит?
        {
            int sumNS = 0, sumEW = 0; //сумма пиковых карт

            Quarters q;
            for (CardValue cv = CardValue.Two; cv <= CardValue.Ace; cv++)
            {
                q = cd.Get(cv, CardSuit.Spades);

                if (q == Quarters.N || q == Quarters.S)
                    sumNS++;
                else if (q == Quarters.E || q == Quarters.W)
                    sumEW++;
            }

            if (sumNS >= 8)
                return Pairs.NS;
            else if (sumEW >= 8)
                return Pairs.EW;
            else
                return Pairs.NotDefinedYet;
        }

        // определить по картам сильную пару (при 20PC!!!)
        static public Pairs DefineStrongest(CardsDistribution cd)
        {
            //При 20 РС компенсация (как прибавление к очкам) начисляется линии, имеющей младший фит, а при двух фитах, не имеющей пикового.

            int fits_NS = CountFits(cd, Pairs.NS, false);
            int fits_EW = CountFits(cd, Pairs.EW, false);

            if (fits_NS > fits_EW)
            {
                return Pairs.NS;
            }
            else if (fits_EW > fits_NS)
            {
                return Pairs.EW;
            }
            else
            {
                if (fits_NS == 0) //обе пары не имеют фитов, => никто не сильнее
                {
                    return Pairs.NotDefinedYet;
                }
                else if (fits_NS == 1) //обе пары по 1 фиту, => сильнее тот, у кого сильнее фит
                {
                    return Whose1FitStronger(cd);
                }
                else if (fits_NS == 2) //обе пары по 2 фита, => сильнее тот, у кого есть пиковый фит
                {
                    return WhoHasSpadesFit(cd);
                }
                else
                    return Pairs.NotDefinedYet;
            }
        }

        // Если 20 фигур - спросить, какая пара сильнее?
        static public void WhoIsStrongest20(BoolData out_strongest, IntData figs, CardsDistribution cd, bool loading_from_db)
        {
            // Если известно распределение карт, то определить по картам сильную пару
            if (cd != null && cd.IsDefined())
            {
                int figs_NS = CountFigurs(cd, Pairs.NS);
                if (figs_NS == 20)
                {
                    Pairs pairStrongest = DefineStrongest(cd);
                    if (pairStrongest == Pairs.NS || pairStrongest == Pairs.NotDefinedYet)
                        out_strongest.Value = true;
                    else
                        out_strongest.Value = false;
                }
                else
                {
                    out_strongest.Born = false;
                }
            }
            // Иначе спросить у юзера
            else if (!loading_from_db)  // ЕСЛИ ИДЕТ ЗАГРУЗКА ИЗ БД - НЕ СПРАШИВАТЬ!!! САМО ЗАГРУЗИТ!!!  !!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                if (figs != null && figs.IsDefined() && figs.Value == 20)
                {
                    if (out_strongest.Born == false) // ВАЖНО!!! СПРАШИВАТЬ ТОЛЬКО ПРИ ИЗМЕНЕНИИ ФИГУР С XX НА 20
                    {
                        if (MessageBox.Show("Какая пара сильнее?\nNS - да, EW - нет", "Фигур 20", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            out_strongest.Value = true;
                        else
                            out_strongest.Value = false;
                    }
                }
                else
                {
                    out_strongest.Born = false;
                }
            }
        }

        // Очки за фиты (ОТНОСИТЕЛЬНО СИЛЬНОЙ ПАРЫ)
        static public void FitsPoints(FitsSwitcher out_fits, CardsDistribution cd, bool TenCardsIsTwoFits)
        {
            if (out_fits == null)
                return;

            // Определить из распределения карт, если оно есть:
            if (cd != null && cd.IsDefined())
            {
                Pairs pairStrongest = Pairs.NotDefinedYet;
                int figs_NS = CountFigurs(cd, Pairs.NS);
                if (figs_NS > 20)
                    pairStrongest = Pairs.NS;
                else if (figs_NS < 20)
                    pairStrongest = Pairs.EW;
                else
                    pairStrongest = DefineStrongest(cd);

                int fitsOfStrongPair = CountFits(cd, pairStrongest, TenCardsIsTwoFits);
                out_fits.Choise = (fitsOfStrongPair > 2 ? 2 : fitsOfStrongPair);
            }
            else
            {

            }
        }

        //Компенсация  !!!!! С ФИТАМИ !!!!!
        static public void GetCompensation_WithFits(SimpleScore out_comp, CardsDistribution cd, IntData fig, ZoneSwitcher zone, bool useReletivePair, PairSwitcher pair, /*fits:*/ FitsSwitcher fits, BoolData strongest, bool TenCardsIsTwoFits, bool LessCompFor2Fits23PC)
        {
            if (out_comp == null)
                return;

            if (zone != null && zone.IsDefined() && cd != null && cd.IsDefined()) //распределение приоритетнее, чем поле "фигуры" и "фиты"
            {
                Pairs pairStrongest = Pairs.NotDefinedYet;
                int figs_NS = CountFigurs(cd, Pairs.NS);
                int figs_EW = 40 - figs_NS;

                if(figs_NS > figs_EW)
                    pairStrongest = Pairs.NS;
                else if(figs_NS < figs_EW)
                    pairStrongest = Pairs.EW;
                else
                    pairStrongest = DefineStrongest(cd);

                int fitsOfStrongPair = CountFits(cd, pairStrongest, TenCardsIsTwoFits);

                int iNS_comp = 0, iEW_comp = 0;
                if(pairStrongest == Pairs.NS)
                    iNS_comp = Compens_Europe(figs_NS, fitsOfStrongPair, ZoneSwitcher.IsPairInZone(Pairs.NS, zone.Zone), LessCompFor2Fits23PC);
                else
                    iEW_comp = Compens_Europe(figs_EW, fitsOfStrongPair, ZoneSwitcher.IsPairInZone(Pairs.EW, zone.Zone), LessCompFor2Fits23PC);
                out_comp.SetScore(iEW_comp, iNS_comp);
            }
            else if (zone != null && zone.IsDefined() && fig != null && fig.IsDefined() && (pair != null && pair.IsDefined() || useReletivePair) && fits != null && fits.IsDefined() && (fig.Value != 20 || fig.Value == 20 && strongest != null && strongest.IsDefined()))
            {
                int NS_figs, EW_figs;
                if (!useReletivePair && pair.Pair == Pairs.NS || useReletivePair && RelativePair == Pairs.NS) //!!!!
                {
                    NS_figs = fig.Value;
                    EW_figs = 40 - NS_figs;
                }
                else
                {
                    EW_figs = fig.Value;
                    NS_figs = 40 - EW_figs;
                }

                Pairs pairStrongest = Pairs.NotDefinedYet;
                if(NS_figs > EW_figs)
                    pairStrongest = Pairs.NS;
                else if(NS_figs < EW_figs)
                    pairStrongest = Pairs.EW;
                else
                    pairStrongest = (strongest.Value ? Pairs.NS : Pairs.EW);

                int fitsOfStrongPair = fits.Choise;

                int iNS_comp = 0, iEW_comp = 0;
                if(pairStrongest == Pairs.NS)
                    iNS_comp = Compens_Europe(NS_figs, fitsOfStrongPair, ZoneSwitcher.IsPairInZone(Pairs.NS, zone.Zone), LessCompFor2Fits23PC);
                else
                    iEW_comp = Compens_Europe(EW_figs, fitsOfStrongPair, ZoneSwitcher.IsPairInZone(Pairs.EW, zone.Zone), LessCompFor2Fits23PC);
                out_comp.SetScore(iEW_comp, iNS_comp);
            }
            else
            {
                out_comp.Born = false;
            }
        }


        //Компенсация
        public enum TypeOfCompensation { Moscow, Europe, Chicago, Milton_York };
        delegate int CompensFunction(int figs, bool zone);
        static public void GetCompensation(SimpleScore out_comp, CardsDistribution cd, IntData fig, ZoneSwitcher zone, bool useReletivePair, PairSwitcher pair, int type_of_compens)
        {
            if (out_comp == null)
                return;

            // Определить тип компенсации и соотв. функцию
            TypeOfCompensation comp_type = (TypeOfCompensation)type_of_compens;
            CompensFunction comp_f;
            switch (comp_type)
            {
                case TypeOfCompensation.Chicago:
                    comp_f = Compens_Chicago;
                    break;
                case TypeOfCompensation.Moscow:
                    comp_f = Compens_Moscow;
                    break;
                case TypeOfCompensation.Milton_York:
                    comp_f = Compens_MiltonYork;
                    break;
                default:
                    throw new Exception("Не могу определить тип компесации: " + type_of_compens.ToString());
            }


            if (zone != null && zone.IsDefined() && cd != null && cd.IsDefined()) //распределение приоритетнее, чем поле "фигуры"
            {
                int NS_figs, EW_figs;
                NS_figs = CountFigurs(cd, Pairs.NS);
                EW_figs = 40 - NS_figs;

                int iNS_comp = comp_f(NS_figs, ZoneSwitcher.IsPairInZone(Pairs.NS, zone.Zone));
                int iEW_comp = comp_f(EW_figs, ZoneSwitcher.IsPairInZone(Pairs.EW, zone.Zone));
                out_comp.SetScore(iEW_comp, iNS_comp);
            }
            else if (zone != null && zone.IsDefined() && fig != null && fig.IsDefined() && (pair != null && pair.IsDefined() || useReletivePair))
            {
                int NS_figs, EW_figs;
                if (!useReletivePair && pair.Pair == Pairs.NS || useReletivePair && RelativePair == Pairs.NS) //!!!!
                {
                    NS_figs = fig.Value;
                    EW_figs = 40 - NS_figs;
                }
                else
                {
                    EW_figs = fig.Value;
                    NS_figs = 40 - EW_figs;
                }
                int iNS_comp = comp_f(NS_figs, ZoneSwitcher.IsPairInZone(Pairs.NS, zone.Zone));
                int iEW_comp = comp_f(EW_figs, ZoneSwitcher.IsPairInZone(Pairs.EW, zone.Zone));
                out_comp.SetScore(iEW_comp, iNS_comp);
            }
            else
            {
                out_comp.Born = false;
            }
        }

        static public int Compens_Europe(int figs, int fits, bool zone, bool LessCompFor2Fits23PC)
        {
            // http://www.aha.ru/~zelmun/kompensa.htm

            if(figs < 20)
                return 0;
            else if(figs == 20)
                return !zone ? (fits == 0 ? 0 : (fits == 1 ? 80 : 110)) : (fits == 0 ? 0 : (fits == 1 ? 80 : 110));
            else if(figs == 21)
                return !zone ? (fits == 0 ? 70 : (fits == 1 ? 90 : 120)) : (fits == 0 ? 70 : (fits == 1 ? 90 : 120));
            else if(figs == 22)
                return !zone ? (fits == 0 ? 80 : (fits == 1 ? 100 : 130)) : (fits == 0 ? 80 : (fits == 1 ? 100 : 130));
            else if(figs == 23)
                return !zone ? (fits == 0 ? 100 : (fits == 1 ? 110 : (LessCompFor2Fits23PC ? 130 : 400))) : (fits == 0 ? 100 : (fits == 1 ? 110 : (LessCompFor2Fits23PC ? 130 : 600)));
            else if(figs == 24)
                return !zone ? (fits == 0 ? 110 : (fits == 1 ? 120 : 400)) : (fits == 0 ? 110 : (fits == 1 ? 120 : 600));
            else if(figs == 25)
                return !zone ? (fits == 0 ? 120 : (fits == 1 ? 400 : 410)) : (fits == 0 ? 120 : (fits == 1 ? 600 : 610));
            else if(figs == 26)
                return !zone ? (fits == 0 ? 400 : (fits == 1 ? 410 : 420)) : (fits == 0 ? 600 : (fits == 1 ? 610 : 610));
            else if(figs == 27)
                return !zone ? (fits == 0 ? 410 : (fits == 1 ? 420 : 430)) : (fits == 0 ? 610 : (fits == 1 ? 620 : 620));
            else if(figs == 28)
                return !zone ? (fits == 0 ? 420 : (fits == 1 ? 430 : 460)) : (fits == 0 ? 620 : (fits == 1 ? 630 : 630));
            else if(figs == 29)
                return !zone ? (fits == 0 ? 430 : (fits == 1 ? 450 : 460)) : (fits == 0 ? 630 : (fits == 1 ? 650 : 660));
            else if(figs == 30)
                return !zone ? (fits == 0 ? 440 : (fits == 1 ? 460 : 960)) : (fits == 0 ? 640 : (fits == 1 ? 660 : 1410));
            else if(figs == 31)
                return !zone ? (fits == 0 ? 450 : (fits == 1 ? 960 : 980)) : (fits == 0 ? 650 : (fits == 1 ? 1410 : 1410));
            else if(figs == 32)
                return !zone ? (fits == 0 ? 460 : (fits == 1 ? 980 : 980)) : (fits == 0 ? 660 : (fits == 1 ? 1430 : 1430));
            else if(figs == 33)
                return !zone ? (fits == 0 ? 960 : (fits == 1 ? 980 : 1480)) : (fits == 0 ? 1410 : (fits == 1 ? 1430 : 1430));
            else if(figs == 34)
                return !zone ? (fits == 0 ? 980 : (fits == 1 ? 1480 : 1480)) : (fits == 0 ? 1430 : (fits == 1 ? 2180 : 2180));
            else if(figs == 35)
                return !zone ? (fits == 0 ? 980 : (fits == 1 ? 1480 : 1480)) : (fits == 0 ? 1430 : (fits == 1 ? 2180 : 2180));
            else //36+
                return !zone ? (fits == 0 ? 1480 : (fits == 1 ? 1480 : 1480)) : (fits == 0 ? 2180 : (fits == 1 ? 2180 : 2180));
        }

        static public int Compens_Chicago(int figs, bool zone)
        {
            // http://bridge-preferance.ru/BridgeRules2.htm

            if (figs <= 20)
                return 0;
            else if (figs == 21)
                return !zone ? 50 : 50;
            else if (figs == 22)
                return !zone ? 70 : 70;
            else if (figs == 23)
                return !zone ? 110 : 110;
            else if (figs == 24)
                return !zone ? 200 : 290;
            else if (figs == 25)
                return !zone ? 300 : 440;
            else if (figs == 26)
                return !zone ? 350 : 520;
            else if (figs == 27)
                return !zone ? 400 : 600;
            else if (figs == 28)
                return !zone ? 430 : 630;
            else if (figs == 29)
                return !zone ? 460 : 660;
            else if (figs == 30)
                return !zone ? 490 : 690;
            else if (figs == 31)
                return !zone ? 600 : 900;
            else if (figs == 32)
                return !zone ? 700 : 1050;
            else if (figs == 33)
                return !zone ? 900 : 1350;
            else if (figs == 34)
                return !zone ? 1000 : 1500;
            else if (figs == 35)
                return !zone ? 1100 : 1650;
            else if (figs == 36)
                return !zone ? 1200 : 1800;
            else //37+
                return !zone ? 1300 : 1950;
        }

        static public int Compens_Moscow(int figs, bool zone)
        {
            // http://www.aha.ru/~zelmun/kompensa.htm

            if (figs <= 20)
                return 0;
            else if (figs == 21)
                return !zone ? 50 : 50;
            else if (figs == 22)
                return !zone ? 70 : 70;
            else if (figs == 23)
                return !zone ? 110 : 110;
            else if (figs == 24)
                return !zone ? 200 : 200;
            else if (figs == 25)
                return !zone ? 300 : 440;
            else if (figs == 26)
                return !zone ? 350 : 520;
            else if (figs == 27)
                return !zone ? 400 : 600;
            else if (figs == 28)
                return !zone ? 430 : 630;
            else if (figs == 29)
                return !zone ? 460 : 660;
            else if (figs == 30)
                return !zone ? 490 : 690;
            else if (figs == 31)
                return !zone ? 600 : 900;
            else if (figs == 32)
                return !zone ? 750 : 1050;
            else if (figs == 33)
                return !zone ? 900 : 1350;
            else if (figs == 34)
                return !zone ? 1000 : 1500;
            else if (figs == 35)
                return !zone ? 1100 : 1650;
            else //36+
                return !zone ? 1200 : 1800;
        }

        static public int Compens_MiltonYork(int figs, bool zone)
        {
            // http://bridge.fromru.com/boykov.html

            if (figs <= 20)
                return 0;
            else if (figs == 21)
                return !zone ? 50 : 50;
            else if (figs == 22)
                return !zone ? 70 : 70;
            else if (figs == 23)
                return !zone ? 110 : 110;
            else if (figs == 24)
                return !zone ? 200 : 200;
            else if (figs == 25)
                return !zone ? 300 : 440;
            else if (figs == 26)
                return !zone ? 350 : 520;
            else if (figs == 27)
                return !zone ? 400 : 600;
            else if (figs == 28)
                return !zone ? 430 : 630;
            else if (figs == 29)
                return !zone ? 460 : 660;
            else if (figs == 30)
                return !zone ? 490 : 690;
            else if (figs == 31)
                return !zone ? 600 : 800;
            else if (figs == 32)
                return !zone ? 750 : 1150;
            else if (figs == 33)
                return !zone ? 900 : 1350;
            else if (figs == 34)
                return !zone ? 1000 : 1500;
            else if (figs == 35)
                return !zone ? 1100 : 1650;
            else if (figs == 36) 
                return !zone ? 1200 : 1800;
            else if (figs == 37)
                return !zone ? 1400 : 2100;
            else if (figs == 38)
                return !zone ? 1430 : 2130;
            else if (figs == 39)
                return !zone ? 1460 : 2160;
            else //40
                return !zone ? 1490 : 2190;
        }


        // IMP
        static public void ConvertToIMPs(SimpleScore out_imp, SimpleScore score)
        {
            if (out_imp == null)
                return;

            if (score == null || !score.IsDefined())
            {
                out_imp.Born = false;
            }
            else
            {
                int NS_IMP = IMP(score.Score.NS);
                int EW_IMP = IMP(score.Score.EW);
                out_imp.SetScore(NS_IMP, EW_IMP);
            }
        }

        static public int IMP(int v)
        {
            if (v < 20)
                return 0;
            if (v >= 20 && v <= 40)
                return 1;
            if (v >= 50 && v <= 80)
                return 2;
            if (v >= 90 && v <= 120)
                return 3;
            if (v >= 130 && v <= 160)
                return 4;
            if (v >= 170 && v <= 210)
                return 5;
            if (v >= 220 && v <= 260)
                return 6;
            if (v >= 270 && v <= 310)
                return 7;
            if (v >= 320 && v <= 360)
                return 8;
            if (v >= 370 && v <= 420)
                return 9;
            if (v >= 430 && v <= 490)
                return 10;
            if (v >= 500 && v <= 590)
                return 11;
            if (v >= 600 && v <= 740)
                return 12;
            if (v >= 750 && v <= 890)
                return 13;
            if (v >= 900 && v <= 1090)
                return 14;
            if (v >= 1100 && v <= 1290)
                return 15;
            if (v >= 1300 && v <= 1490)
                return 16;
            if (v >= 1500 && v <= 1740)
                return 17;
            if (v >= 1750 && v <= 1990)
                return 18;
            if (v >= 2000 && v <= 2240)
                return 19;
            if (v >= 2250 && v <= 2490)
                return 20;
            if (v >= 2500 && v <= 2990)
                return 21;
            if (v >= 3000 && v <= 3490)
                return 22;
            if (v >= 3500 && v <= 3990)
                return 23;
            if (v >= 4000)
                return 24;

            return 0;
        }

        static public Dictionary<int, ArrayOfInt> VP_Table;
        static public int vp_total = 30; //кол-во VP, которые разыгрываются = VP_Table[dealsInMatch].Count * 2
        static public int vp_max = 25; //максимум VP на игрока
        static BridgeGameScoring()
        {
            // Инициализация VP-таблицы:
            VP_Table = new Dictionary<int, ArrayOfInt>();
            VP_Table.Add(8, new ArrayOfInt(new int[]  {  1, 5,  8,  11, 14, 17, 20, 23, 26, 29, 33, 37, 41, 45, 50  }));
            VP_Table.Add(12, new ArrayOfInt(new int[] {  1, 6,  9,  12, 16, 20, 24, 28, 32, 36, 40, 45, 50, 55, 61  }));
            VP_Table.Add(16, new ArrayOfInt(new int[] {  2, 7,  11, 15, 19, 23, 27, 31, 36, 41, 46, 52, 58, 64, 71  }));
            VP_Table.Add(20, new ArrayOfInt(new int[] {  2, 8,  12, 16, 21, 26, 31, 36, 41, 47, 53, 59, 65, 72, 79  }));
            VP_Table.Add(24, new ArrayOfInt(new int[] {  3, 9,  14, 19, 24, 29, 34, 39, 45, 51, 57, 64, 71, 79, 87  }));
            VP_Table.Add(28, new ArrayOfInt(new int[] {  3, 10, 15, 20, 25, 31, 37, 43, 49, 55, 61, 68, 76, 85, 94  }));
            VP_Table.Add(32, new ArrayOfInt(new int[] {  3, 10, 16, 22, 28, 34, 40, 46, 52, 58, 65, 73, 82, 91, 100 }));

            /*
            ----T---T---T---T---T---T---T---T---T---T---T---T---T---T---T---¬
            ¦VP:¦15 ¦14 ¦13 ¦12 ¦11 ¦10 ¦ 9 ¦ 8 ¦ 7 ¦ 6 ¦ 5 ¦ 4 ¦ 3 ¦ 2 ¦ 1 ¦
            ¦   ¦15 ¦16 ¦17 ¦18 ¦19 ¦20 ¦21 ¦22 ¦23 ¦24 ¦25 ¦25 ¦25 ¦25 ¦25 ¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦ 8:¦0-1¦2-5¦6-8¦-11¦-14¦-17¦-20¦-23¦-26¦-29¦-33¦-37¦-41¦-45¦-50¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦12:¦0-1¦2-6¦7-9¦-12¦-16¦-20¦-24¦-28¦-32¦-36¦-40¦-45¦-50¦-55¦-61¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦16:¦0-2¦3-7¦-11¦-15¦-19¦-23¦-27¦-31¦-36¦-41¦-46¦-52¦-58¦-64¦-71¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦20:¦0-2¦3-8¦-12¦-16¦-21¦-26¦-31¦-36¦-41¦-47¦-53¦-59¦-65¦-72¦-79¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦24:¦0-3¦4-9¦-14¦-19¦-24¦-29¦-34¦-39¦-45¦-51¦-57¦-64¦-71¦-79¦-87¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦28:¦0-3¦-10¦-15¦-20¦-25¦-31¦-37¦-43¦-49¦-55¦-61¦-68¦-76¦-85¦-94¦
            +---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+
            ¦32:¦0-3¦-10¦-16¦-22¦-28¦-34¦-40¦-46¦-52¦-58¦-65¦-73¦-82¦-91¦-100
            L---+---+---+---+---+---+---+---+---+---+---+---+---+---+---+----
                   ПРИМ: Превышение указанных в таблице  сумм  дает  итоговый
                         счет встречи 25:0.
            */
        }
        static public DealScore IMPtoVP(DealScore imp, int dealsInMatch)
        {
            if (!VP_Table.Keys.Contains(dealsInMatch))
            {
                // MessageBox.Show("В таблице VP нет данных для " + dealsInMatch + " сдач в матче");
                return new DealScore(0, 0);
            }
            
            int diff = imp.NS - imp.EW;
            int d = (diff < 0 ? -diff : diff); //разница по модулю

            int vp_pos = VP_Table[dealsInMatch].Count;
            for (int i = 0; i < VP_Table[dealsInMatch].Count; i++)
            {
                if (d <= (VP_Table[dealsInMatch])[i])
                {
                    vp_pos = i;
                    break;
                }
            }

            int vp1 = (vp_total / 2) + vp_pos;
            int vp2 = (vp_total / 2) - vp_pos;
            if (vp1 > vp_max)
                vp1 = vp_max;

            if (diff >= 0) //NS выиграл
                return new DealScore(vp1, vp2);
            else //EW выиграл
                return new DealScore(vp2, vp1);
        }


        // Подсчёт очков в робберном бридже
        static public void RobberScore(DealScoreExt out_score_ext, PairSwitcher pair, ZoneSwitcher zone, Contract contract, Result result, bool BonusForWholeRobber, Robber rob, int robdealNo)
        {
            RobberScore(out_score_ext, pair, zone, contract, result, BonusForWholeRobber, rob, robdealNo, false, null);
        }
        static public void RobberScore(DealScoreExt out_score_ext, PairSwitcher pair, ZoneSwitcher zone, Contract contract, Result result, bool BonusForWholeRobber, Robber rob, int robdealNo, OnersSwitcher oners)
        {
            RobberScore(out_score_ext, pair, zone, contract, result, BonusForWholeRobber, rob, robdealNo, true, oners);
        }
        static public void RobberScore(DealScoreExt out_score_ext, PairSwitcher pair, ZoneSwitcher zone, Contract contract, Result result, bool BonusForWholeRobber, Robber rob, int robdealNo, bool useOners, OnersSwitcher oners)
        {
            if (rob.WhereCompleted() != -1 && rob.WhereCompleted() < robdealNo)
            {
                out_score_ext.Clear();
            }
            else if (contract.NoContract)
            {
                out_score_ext.DealWasNotPlayed();
            }
            else if (!pair.IsDefined() || !zone.IsDefined() || !contract.IsDefined() || !result.IsDefined() || (useOners && !oners.IsDefined()))
            {
                out_score_ext.Clear();
            }
            else
            {
                bool inzone = ZoneSwitcher.IsPairInZone(pair.Pair, zone.Zone);
                bool enemy_inzone = ZoneSwitcher.IsPairInZone(pair.Pair == Pairs.EW ? Pairs.NS : Pairs.EW, zone.Zone);
                if (result.Quantity >= 0)
                {
                    // --- Если контракт сыгран ---
                    // Очки за взятки:
                    int score = contract.Quantity * (SmallHelper.WhatTrumpType(contract.Trump) == TrumpType.Minor ? 20 : 30) + (contract.Trump == CardTrump.NT ? 10 : 0);
                    if (contract.Contra)
                        score *= 2;
                    else if (contract.ReContra)
                        score *= 4;
                    // Премия за превышение взяток:
                    ArrayList bonuses = new ArrayList();
                    if (result.Quantity > 0)
                    {
                        if (contract.Contra)
                            bonuses.Add(result.Quantity * (inzone ? 200 : 100));
                        else if (contract.ReContra)
                            bonuses.Add(result.Quantity * (inzone ? 400 : 200));
                        else
                            bonuses.Add(result.Quantity * (SmallHelper.WhatTrumpType(contract.Trump) == TrumpType.Minor ? 20 : 30));
                    }
                    if (contract.Quantity == 6) //малый шлем
                    {
                        bonuses.Add(inzone ? 750 : 500);
                    }
                    else if (contract.Quantity == 7) //большой шлем
                    {
                        bonuses.Add(inzone ? 1500 : 1000);
                    }
                    // Дополнительная премия за оскорбление:
                    if (contract.Contra)
                        bonuses.Add(50);
                    else if (contract.ReContra)
                        bonuses.Add(100);
 
                    switch (pair.Pair)
                    {
                        case Pairs.EW:
                            out_score_ext.EW_down = score;

                            // Премия за роббер:
                            if (BonusForWholeRobber)
                            {
                                if (rob.WhereCompleted() == robdealNo)
                                    bonuses.Add(enemy_inzone ? 500 : 700);
                            }
                            // Премия за гейм:
                            else
                            {
                                if (rob.WhereCompleted() == robdealNo)
                                    bonuses.Add(500);
                                else if (rob.DoesDealMakeGame(robdealNo))
                                    bonuses.Add(200);
                            }

                            out_score_ext.EW_up = new int[bonuses.Count];
                            for (int i = 0; i < bonuses.Count; i++)
                                out_score_ext.EW_up[i] = (int)bonuses[i];
                            out_score_ext.NS_down = 0;
                            out_score_ext.NS_up = null;
                            break;
                        case Pairs.NS:
                            out_score_ext.NS_down = score;

                            // Премия за роббер:
                            if (BonusForWholeRobber)
                            {
                                if (rob.WhereCompleted() == robdealNo)
                                    bonuses.Add(enemy_inzone ? 500 : 700);
                            }
                            // Премия за гейм:
                            else
                            {
                                if (rob.WhereCompleted() == robdealNo)
                                    bonuses.Add(500);
                                else if (rob.DoesDealMakeGame(robdealNo))
                                    bonuses.Add(200);
                            }

                            out_score_ext.NS_up = new int[bonuses.Count];
                            for (int i = 0; i < bonuses.Count; i++)
                                out_score_ext.NS_up[i] = (int)bonuses[i];
                            out_score_ext.EW_down = 0;
                            out_score_ext.EW_up = null;
                            break;
                    }
                }
                else
                {
                    // --- Если контракт проигран ---
                    int enemy_score = 0;
                    for (int i = -1; i >= result.Quantity; i--)
                    {
                        if (i == -1)
                        {
                            if (contract.Contra)
                                enemy_score += (inzone ? 200 : 100);
                            else if (contract.ReContra)
                                enemy_score += (inzone ? 400 : 200);
                            else
                                enemy_score += (inzone ? 100 : 50);
                        }
                        else
                        {
                            if (contract.Contra)
                                enemy_score += (inzone ? 300 : 200);
                            else if (contract.ReContra)
                                enemy_score += (inzone ? 600 : 400);
                            else
                                enemy_score += (inzone ? 100 : 50);
                        }
                    }
                    switch (pair.Pair)
                    {
                        case Pairs.EW:
                            out_score_ext.EW_down = 0;
                            out_score_ext.EW_up = null;
                            out_score_ext.NS_down = 0;
                            out_score_ext.NS_up = new int[1] { enemy_score };
                            break;
                        case Pairs.NS:
                            out_score_ext.NS_down = 0;
                            out_score_ext.NS_up = null;
                            out_score_ext.EW_down = 0;
                            out_score_ext.EW_up = new int[1] { enemy_score };
                            break;
                    }
                }


                // Премия за онеры !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if (useOners && oners.Choise != 0)
                {
                    int bonus_for_oners = 0;
                    if (oners.Choise == 1 || oners.Choise == 4) //4o
                        bonus_for_oners = 100;
                    else if (oners.Choise == 2 || oners.Choise == 5) //5o
                        bonus_for_oners = 150;
                    else if (oners.Choise == 3 || oners.Choise == 6) //4A
                        bonus_for_oners = 150;

                    if (oners.Choise >= 1 && oners.Choise <= 3) //премия NS
                    {
                        if (out_score_ext.NS_up == null)
                            out_score_ext.NS_up = new int[] { bonus_for_oners };
                        else
                        {
                            int[] old = out_score_ext.NS_up;
                            out_score_ext.NS_up = new int[old.Count() + 1];
                            for (int i = 0; i < old.Count(); i++)
                                out_score_ext.NS_up[i] = old[i];
                            out_score_ext.NS_up[old.Count()] = bonus_for_oners;
                        }
                    }
                    else if (oners.Choise >= 4 && oners.Choise <= 6) //премия EW
                    {
                        if (out_score_ext.EW_up == null)
                            out_score_ext.EW_up = new int[] { bonus_for_oners };
                        else
                        {
                            int[] old = out_score_ext.EW_up;
                            out_score_ext.EW_up = new int[old.Count() + 1];
                            for (int i = 0; i < old.Count(); i++)
                                out_score_ext.EW_up[i] = old[i];
                            out_score_ext.EW_up[old.Count()] = bonus_for_oners;
                        }
                    }
                }
            }
        }

        static public void RobberScoreLight(SimpleScore out_score, DealScoreExt score_ext)
        {
            if (score_ext.IsEmpty())
            {
                out_score.Born = false;
            }
            else
            {
                {
                    int iNS = score_ext.NS_down;
                    int iEW = score_ext.EW_down;
                    if(score_ext.NS_up != null)
                    {
                        for (int i = 0; i < score_ext.NS_up.Length; i++)
                        {
                            iNS += score_ext.NS_up[i];
                        }
                    }
                    if (score_ext.EW_up != null)
                    {
                        for (int i = 0; i < score_ext.EW_up.Length; i++)
                        {
                            iEW += score_ext.EW_up[i];
                        }
                    }                    
                    out_score.SetScore(iNS, iEW);
                }
            }
        }

        // Подсчёт очков в спортивном бридже
        static public void SportScoreInvert(SimpleScore out_score, PairSwitcher pair, ZoneSwitcher zone, Contract contract, Result result)
        {
            SportScore(out_score, pair, zone, contract, result, true);
        }
        static public void SportScore(SimpleScore out_score, PairSwitcher pair, ZoneSwitcher zone, Contract contract, Result result)
        {
            SportScore(out_score, pair, zone, contract, result, false);
        }
        static public void SportScore(SimpleScore out_score, PairSwitcher pair, ZoneSwitcher zone, Contract contract, Result result, bool bInvertScore)
        {
            if (out_score == null)
                return;

            if (contract.NoContract)
            {
                out_score.SetScore(0, 0);
            }
            else if (pair == null || zone == null || contract == null || result == null || !pair.IsDefined() || !zone.IsDefined() || !contract.IsDefined() || !result.IsDefined())
            {
                out_score.Born = false;
            }
            else
            {
                bool inzone = ZoneSwitcher.IsPairInZone(pair.Pair, zone.Zone);
                int iNS = 0, iEW = 0;
                if (result.Quantity >= 0)
                {
                    // --- Если контракт сыгран ---
                    // Очки за взятки:
                    int score = contract.Quantity * (SmallHelper.WhatTrumpType(contract.Trump) == TrumpType.Minor ? 20 : 30) + (contract.Trump == CardTrump.NT ? 10 : 0);
                    if (contract.Contra)
                        score *= 2;
                    else if (contract.ReContra)
                        score *= 4;
                    // Премия за превышение взяток:
                    int bonus = 0;
                    if (result.Quantity > 0)
                    {
                        if (contract.Contra)
                            bonus += result.Quantity * (inzone ? 200 : 100);
                        else if (contract.ReContra)
                            bonus += result.Quantity * (inzone ? 400 : 200);
                        else
                            bonus += result.Quantity * (SmallHelper.WhatTrumpType(contract.Trump) == TrumpType.Minor ? 20 : 30);
                    }
                    if (contract.Quantity == 6) //малый шлем
                    {
                        bonus += (inzone ? 750 : 500);
                    }
                    if (contract.Quantity == 7) //большой шлем
                    {
                        bonus += (inzone ? 1500 : 1000);
                    }
                    if (score >= 100) //за гейм
                    {
                        bonus += (inzone ? 500 : 300);
                    }
                    else //за частичную запись
                    {
                        bonus += 50;
                    }
                    // Дополнительная премия:
                    int bonus_plus = 0;
                    if (contract.Contra)
                        bonus_plus = 50;
                    else if (contract.ReContra)
                        bonus_plus = 100;

                    // Присвоение очков:
                    if(pair.Pair == Pairs.NS && !bInvertScore || pair.Pair == Pairs.EW && bInvertScore)
                        iNS = score + bonus + bonus_plus;
                    else if(pair.Pair == Pairs.EW && !bInvertScore || pair.Pair == Pairs.NS && bInvertScore)
                        iEW = score + bonus + bonus_plus;
                }
                else
                {
                    // --- Если контракт проигран ---
                    int enemy_score = 0;
                    for (int i = -1; i >= result.Quantity; i--)
                    {
                        if (i == -1)
                        {
                            if (contract.Contra)
                                enemy_score += (inzone ? 200 : 100);
                            else if (contract.ReContra)
                                enemy_score += (inzone ? 400 : 200);
                            else
                                enemy_score += (inzone ? 100 : 50);
                        }
                        else if (i == -2 || i == -3)
                        {
                            if (contract.Contra)
                                enemy_score += (inzone ? 300 : 200);
                            else if (contract.ReContra)
                                enemy_score += (inzone ? 600 : 400);
                            else
                                enemy_score += (inzone ? 100 : 50);
                        }
                        else
                        {
                            if (contract.Contra)
                                enemy_score += (inzone ? 300 : 300);
                            else if (contract.ReContra)
                                enemy_score += (inzone ? 600 : 600);
                            else
                                enemy_score += (inzone ? 100 : 50);
                        }
                    }

                    // Присвоение очков:
                    if (pair.Pair == Pairs.NS && !bInvertScore || pair.Pair == Pairs.EW && bInvertScore)
                        iEW = enemy_score;
                    else if (pair.Pair == Pairs.EW && !bInvertScore || pair.Pair == Pairs.NS && bInvertScore)
                        iNS = enemy_score;
                }

                // !!! Запись очков в объект данных:
                out_score.SetScore(iNS, iEW);
            }
        }
    }
}
