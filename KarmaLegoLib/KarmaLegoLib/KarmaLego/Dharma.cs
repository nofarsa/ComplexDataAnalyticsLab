using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;


namespace KarmaLegoLib
{
    public class EntsStiListStiDics
    {
        // eIdx, sti, List<sti>
        Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>> entsStiListStiDics;
        public EntsStiListStiDics()
        {
            entsStiListStiDics = new Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>();
        }

        public int getVerticalSupport() { return entsStiListStiDics.Count(); }

        public int horizontalSupport;

        public Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>> getEntsStiListStiDics() { return entsStiListStiDics; }
        
        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getEntityDic(int eIdx) 
        {
            if (entsStiListStiDics.ContainsKey(eIdx))
                return entsStiListStiDics[eIdx];
            return null;
        }

        public List<TimeIntervalSymbol> getTisListbyEIdxTis(int eIdx, TimeIntervalSymbol tis) 
        {
            if (entsStiListStiDics.ContainsKey(eIdx) && entsStiListStiDics[eIdx].ContainsKey(tis))
                return entsStiListStiDics[eIdx][tis]; 
            return null;
        }

        public void indexByEidxSTIs(int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            if (!entsStiListStiDics.ContainsKey(eIdx))
            {
                List<TimeIntervalSymbol> stiList = new List<TimeIntervalSymbol>();
                stiList.Add(tisVal);
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> stiListSti = new Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>();
                stiListSti.Add(tisKey, stiList);
                entsStiListStiDics.Add(eIdx, stiListSti);
            }
            else if (!entsStiListStiDics[eIdx].ContainsKey(tisKey))
            {
                List<TimeIntervalSymbol> stiList = new List<TimeIntervalSymbol>();
                stiList.Add(tisVal);
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> stiListSti = new Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>();
                entsStiListStiDics[eIdx].Add(tisKey, stiList);
            }
            else
                entsStiListStiDics[eIdx][tisKey].Add(tisVal);
            horizontalSupport++;
        }
    }

    public class relVecSymsDic_entsSTIsDics
    {
        Dictionary<int, Dictionary<int, EntsStiListStiDics>>[] relVecSymSym_entSTIsDics; // = new Dictionary<int, Dictionary<int, EntitiesVecOfstiListDic>>();

        public relVecSymsDic_entsSTIsDics(int relStyle)
        {
            relVecSymSym_entSTIsDics = new Dictionary<int, Dictionary<int, EntsStiListStiDics>>[relStyle];
        }

        public int getSymSymVerticalSupport(int frstTndx, int relIdx, int scndTndx)
        {
            if(relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx) && relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                return relVecSymSym_entSTIsDics[relIdx][frstTndx][scndTndx].getVerticalSupport();
            return 0;
        }

        public int getSymSymHorizontalSupport(int frstTndx, int relIdx, int scndTndx)
        {
            if (relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx) && relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                return relVecSymSym_entSTIsDics[relIdx][frstTndx][scndTndx].horizontalSupport;
            return 0;
        }

        public List<TimeIntervalSymbol> getSymSymEidxTisList(int eIdx, int frstTndx, int relIdx, int scndTndx, TimeIntervalSymbol tis)
        {
            if (relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx) && relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                return relVecSymSym_entSTIsDics[relIdx][frstTndx][scndTndx].getTisListbyEIdxTis(eIdx, tis);
            return null;
        }

        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getSymSymEntityDic(int eIdx, int frstTndx, int relIdx, int scndTndx)
        {
            if (relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx) && relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                return relVecSymSym_entSTIsDics[relIdx][frstTndx][scndTndx].getEntityDic(eIdx);
            return null;
        }

        public Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>/*[]*/ getSymSymEntities(int frstTndx, int relIdx, int scndTndx)
        {
            if (relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx) && relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                return relVecSymSym_entSTIsDics[relIdx][frstTndx][scndTndx].getEntsStiListStiDics();
            return null;
        }

        public void indexTTsRelEidxSTIs(int frstTndx, int relIdx, int scndTndx, int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            if (relVecSymSym_entSTIsDics[relIdx] == null)
                relVecSymSym_entSTIsDics[relIdx] = new Dictionary<int, Dictionary<int, EntsStiListStiDics>>();
            if(!relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx))
                relVecSymSym_entSTIsDics[relIdx].Add(frstTndx, new Dictionary<int,EntsStiListStiDics>());
            if (!relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                relVecSymSym_entSTIsDics[relIdx][frstTndx].Add(scndTndx, new EntsStiListStiDics());

            relVecSymSym_entSTIsDics[relIdx][frstTndx][scndTndx].indexByEidxSTIs(eIdx, tisKey, tisVal);
        }
    }

    

    public class relVecSymVecSymDic_entsSTIsDics
    {
        public class Sym2Dic_entsSTIsDics
        {
            public Dictionary<int, EntsStiListStiDics> sym2Dic_entsSTIsDics = new Dictionary<int,EntsStiListStiDics>();
        }

        public class Sym1Vec_Sym2Dic_entsSTIsDics
        {
            public Sym2Dic_entsSTIsDics[] sym1Vec_sym2Dic_entsSTIsDics;

            public Sym1Vec_Sym2Dic_entsSTIsDics(int symSize)
            {
                sym1Vec_sym2Dic_entsSTIsDics = new Sym2Dic_entsSTIsDics[symSize];
            }
        }

        Sym1Vec_Sym2Dic_entsSTIsDics[] relVec_sym1Vec_sym2Dic_entsSTIsDics; // Dictionary<int, Dictionary<int, EntitiesVecOfstiListDic>>[] relVecSymSym_entSTIsDics; // = new Dictionary<int, Dictionary<int, EntitiesVecOfstiListDic>>();

        public relVecSymVecSymDic_entsSTIsDics(int relStyle)
        {
            //relVecSymSym_entSTIsDics = new Dictionary<int, Dictionary<int, EntitiesVecOfstiListDic>>[relStyle];
            relVec_sym1Vec_sym2Dic_entsSTIsDics = new Sym1Vec_Sym2Dic_entsSTIsDics[relStyle];
        }

        public int getSymSymVerticalSupport(int frstTndx, int relIdx, int scndTndx)
        {
            if(relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] != null && relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.ContainsKey(scndTndx))
            //if (relVecSymSym_entSTIsDics[relIdx].ContainsKey(frstTndx) && relVecSymSym_entSTIsDics[relIdx][frstTndx].ContainsKey(scndTndx))
                return relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics[scndTndx].getVerticalSupport();
            return 0;
        }

        public int getSymSymHorizontalSupport(int frstTndx, int relIdx, int scndTndx)
        {
            if (relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] != null && relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.ContainsKey(scndTndx))
                return relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics[scndTndx].horizontalSupport;
            return 0;
        }

        public List<TimeIntervalSymbol> getSymSymEidxTisList(int eIdx, int frstTndx, int relIdx, int scndTndx, TimeIntervalSymbol tis)
        {
            if (relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] != null && relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.ContainsKey(scndTndx))
                return relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics[scndTndx].getTisListbyEIdxTis(eIdx, tis);
            return null;
        }

        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getSymSymEntityDic(int eIdx, int frstTndx, int relIdx, int scndTndx)
        {
            if (relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] != null && relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.ContainsKey(scndTndx))
                return relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics[scndTndx].getEntityDic(eIdx);
            return null;
        }

        public Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>/*[]*/ getSymSymEntities(int frstTndx, int relIdx, int scndTndx)
        {
            if (relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] != null && relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.ContainsKey(scndTndx))
                return relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics[scndTndx].getEntsStiListStiDics(); // .getEntitiesVec();
            return null;
        }

        public void indexTTsRelEidxSTIs(int frstTndx, int relIdx, int scndTndx, int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal, int symSize)
        {
            if (relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx] == null) //.sym1Vec_sym2Dic_entsSTIsDics[frstTndx] == null)
                relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx] = new Sym1Vec_Sym2Dic_entsSTIsDics(symSize);
            if (relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] == null)
                relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx] = new Sym2Dic_entsSTIsDics();
            if (!relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.ContainsKey(scndTndx))
                relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics.Add(scndTndx, new EntsStiListStiDics());

            relVec_sym1Vec_sym2Dic_entsSTIsDics[relIdx].sym1Vec_sym2Dic_entsSTIsDics[frstTndx].sym2Dic_entsSTIsDics[scndTndx].indexByEidxSTIs(eIdx, tisKey, tisVal);
        }

    }

    public class relSymSymDics_entsSTIsDics
    {
        Dictionary<int, Dictionary<int, Dictionary<int, EntsStiListStiDics>>> SymRelSym_EntsSTIsListDic = new Dictionary<int, Dictionary<int, Dictionary<int, EntsStiListStiDics>>>();
        
        public int getSymSymVerticalSupport(int frstTndx, int relIdx, int scndTndx)
        {
            if (SymRelSym_EntsSTIsListDic.ContainsKey(frstTndx) && SymRelSym_EntsSTIsListDic[frstTndx].ContainsKey(relIdx) && SymRelSym_EntsSTIsListDic[frstTndx][relIdx].ContainsKey(scndTndx))
                    return SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getVerticalSupport();
            return 0;
        }

        public int getSymSymHorizontalSupport(int frstTndx, int relIdx, int scndTndx)
        {
            if (SymRelSym_EntsSTIsListDic.ContainsKey(frstTndx) && SymRelSym_EntsSTIsListDic[frstTndx].ContainsKey(relIdx) && SymRelSym_EntsSTIsListDic[frstTndx][relIdx].ContainsKey(scndTndx))
               return SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].horizontalSupport;
            return 0;
        }

        public List<TimeIntervalSymbol> getSymSymEidxTisList(int eIdx, int frstTndx, int relIdx, int scndTndx, TimeIntervalSymbol tis)
        {
            if (SymRelSym_EntsSTIsListDic.ContainsKey(frstTndx) && SymRelSym_EntsSTIsListDic[frstTndx].ContainsKey(relIdx) && SymRelSym_EntsSTIsListDic[frstTndx][relIdx].ContainsKey(scndTndx))
               return SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getTisListbyEIdxTis(eIdx, tis);
            return null;
        }

        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getSymSymEntityDic(int eIdx, int frstTndx, int relIdx, int scndTndx)
        {
            if (SymRelSym_EntsSTIsListDic.ContainsKey(frstTndx) && SymRelSym_EntsSTIsListDic[frstTndx].ContainsKey(relIdx) && SymRelSym_EntsSTIsListDic[frstTndx][relIdx].ContainsKey(scndTndx))
               return SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getEntityDic(eIdx);
            return null;
        }

        
        public Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>/*[]*/ getSymSymEntities/*Vec*/(int frstTndx, int relIdx, int scndTndx)
        {
            if (SymRelSym_EntsSTIsListDic.ContainsKey(frstTndx) && SymRelSym_EntsSTIsListDic[frstTndx].ContainsKey(relIdx) && SymRelSym_EntsSTIsListDic[frstTndx][relIdx].ContainsKey(scndTndx))
            {
                return SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getEntsStiListStiDics();
                /*int entSize = SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getVerticalSupport();
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] entsDicsVec = new Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[entSize];
                for (int i = 0; i < entSize; i++)
                {
                    if(SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getEntsStiListStiDics().ContainsKey(i))
                        entsDicsVec[i] = SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getEntsStiListStiDics()[i];
                }
                return entsDicsVec; // SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].getEntitiesVec();
                 */ 
            }
            return null;
        }

        public void indexTTsRelEidxSTIs(int frstTndx, int relIdx, int scndTndx, int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal)
        {
            if (!SymRelSym_EntsSTIsListDic.ContainsKey(frstTndx))
                SymRelSym_EntsSTIsListDic.Add(frstTndx, new Dictionary<int, Dictionary<int, EntsStiListStiDics>>());
            if (!SymRelSym_EntsSTIsListDic[frstTndx].ContainsKey(relIdx))
                SymRelSym_EntsSTIsListDic[frstTndx].Add(relIdx, new Dictionary<int, EntsStiListStiDics>());
            if (!SymRelSym_EntsSTIsListDic[frstTndx][relIdx].ContainsKey(scndTndx))
                SymRelSym_EntsSTIsListDic[frstTndx][relIdx].Add(scndTndx, new EntsStiListStiDics());

            SymRelSym_EntsSTIsListDic[frstTndx][relIdx][scndTndx].indexByEidxSTIs(eIdx, tisKey, tisVal);
        }

    }

    public class dharmaIndex // like tIndex
    {
        private int dharmaIndexType;
        private relVecSymVecSymDic_entsSTIsDics relVecSymVecSymDicEntsSTIsDics;
        private relVecSymsDic_entsSTIsDics relVecSymsDicEntsSTIsDics; // relEntryOfSymbsDicEntsSTIsDics[] relsVecOfSymbsDicEntsSTIsDics; 
        private relSymSymDics_entsSTIsDics      relSymSymDicsEntsSTIsDics;

        public dharmaIndex(int set_dharmaIndexType, int seTentitieSize, int setRelSize) //, bool setFullDics)
        {
            dharmaIndexType = set_dharmaIndexType;
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic :
                    relVecSymVecSymDicEntsSTIsDics = new relVecSymVecSymDic_entsSTIsDics(setRelSize);
                    break;

                case KLC.dharma_relVecSymSymDics :
                    relVecSymsDicEntsSTIsDics = new relVecSymsDic_entsSTIsDics(setRelSize); // relsVecOfSymbsDicEntsSTIsDics = new relEntryOfSymbsDicEntsSTIsDics[seTrelSize];
                    break;

                case KLC.dharma_relSymSymDics :
                    relSymSymDicsEntsSTIsDics = new relSymSymDics_entsSTIsDics();
                    break;
            }
        }

        public int getSymSymRelVerticalSupport(int frstTndx, int scndTndx, int relIdx) 
        { 
            //return Kindex[frstTndx, scndTndx].relsVecOfEntitiesDics[rel].getVerticalSupport(); 
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic:
                    return relVecSymVecSymDicEntsSTIsDics.getSymSymVerticalSupport(frstTndx, relIdx, scndTndx);
                    break;
                
                case KLC.dharma_relVecSymSymDics:
                    return relVecSymsDicEntsSTIsDics.getSymSymVerticalSupport(frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relSymSymDics:
                    return relSymSymDicsEntsSTIsDics.getSymSymVerticalSupport(frstTndx, relIdx, scndTndx);
                    break;

                default :
                    return 0;
                    break;
            }
        }
        
        public int getSymSymRelHorizontalSupport(int frstTndx, int scndTndx, int relIdx)
        { 
            //return Kindex[frstTndx,scndTndx].relsVecOfEntitiesDics[rel].horizontalSupport; 
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic:
                    return relVecSymVecSymDicEntsSTIsDics.getSymSymHorizontalSupport(frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relVecSymSymDics:
                    return relVecSymsDicEntsSTIsDics.getSymSymHorizontalSupport(frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relSymSymDics:
                    return relSymSymDicsEntsSTIsDics.getSymSymHorizontalSupport(frstTndx, relIdx, scndTndx);
                    break;

                default :
                    return 0;
                    break;
            }
        }

        public List<TimeIntervalSymbol> getTindexRelEidxTisList(int frstTndx, int scndTndx, int relIdx, int eIdx, TimeIntervalSymbol tis)
        { 
            //return Kindex[frstTndx, scndTndx].relsVecOfEntitiesDics[rel].getTisListbyEIdxTis(eIdx, tis); 
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic:
                    return relVecSymVecSymDicEntsSTIsDics.getSymSymEidxTisList(eIdx, frstTndx, relIdx, scndTndx, tis);
                    break;

                case KLC.dharma_relVecSymSymDics:
                    return relVecSymsDicEntsSTIsDics.getSymSymEidxTisList(eIdx, frstTndx, relIdx, scndTndx, tis) ;
                    break;

                case KLC.dharma_relSymSymDics:
                    return relSymSymDicsEntsSTIsDics.getSymSymEidxTisList(eIdx, frstTndx, relIdx, scndTndx, tis);
                    break;

                default :
                    return null;
                    break;
            }
        }

        public Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> getTindexRelEidxDic(int frstTndx, int scndTndx, int relIdx, int eIdx)
        {
            //if (frstTndx > -1 && scndTndx > -1) return Kindex[frstTndx, scndTndx].getRelEntityDic(rel, eIdx); else return null;
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic:
                    return relVecSymVecSymDicEntsSTIsDics.getSymSymEntityDic(eIdx, frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relVecSymSymDics:
                    return relVecSymsDicEntsSTIsDics.getSymSymEntityDic(eIdx, frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relSymSymDics:
                    return relSymSymDicsEntsSTIsDics.getSymSymEntityDic(eIdx, frstTndx, relIdx, scndTndx);
                    break;

                default:
                    return null;
                    break;
            }
        }
        
        public Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>>/*[]*/ getTindexRelEntitiesDicsVec(int frstTndx, int scndTndx, int relIdx) 
        { 
            //return Kindex[frstTndx, scndTndx].getRelEntitiesDics(rel); 
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic:
                    return relVecSymVecSymDicEntsSTIsDics.getSymSymEntities(frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relVecSymSymDics:
                    return relVecSymsDicEntsSTIsDics.getSymSymEntities(frstTndx, relIdx, scndTndx);
                    break;

                case KLC.dharma_relSymSymDics:
                    return relSymSymDicsEntsSTIsDics.getSymSymEntities(frstTndx, relIdx, scndTndx);
                    break;

                default :
                    return null;
                    break;
            }
        }
        
        public void indexTTsRelEidxSTIs(int frstTndx, int scndTndx, int relIdx, int eIdx, TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal, int symSize)
        {
             //Kindex[frstTncptIdx, scndTncptIdx].indexByRelEidxSTIs(rel, eIdx, tisKey, tisVal);
            switch(dharmaIndexType)
            {
                case KLC.dharma_relVecSymVecSymDic:
                    relVecSymVecSymDicEntsSTIsDics.indexTTsRelEidxSTIs(frstTndx, relIdx, scndTndx, eIdx, tisKey, tisVal, symSize);
                    break;

                case KLC.dharma_relVecSymSymDics:
                    relVecSymsDicEntsSTIsDics.indexTTsRelEidxSTIs(frstTndx, relIdx, scndTndx, eIdx, tisKey, tisVal); 
                    break;

                case KLC.dharma_relSymSymDics:
                    relSymSymDicsEntsSTIsDics.indexTTsRelEidxSTIs(frstTndx, relIdx, scndTndx, eIdx, tisKey, tisVal);
                    break;

                default :
                    break;
            }
        }

        public TIRP get2SizedAsTIRP(int tTrgtID, int tErlyID, int tTrgtIdx, int tErlyIdx, int rel)
        {
            TIRP twoSzdTIRP = new TIRP(tTrgtID, tErlyID, rel);
            ////string trK = toncepts[tTrgtID].tonceptINDEX.ToString() + "-" + rel + "-" + toncepts[tErlyID].tonceptINDEX;
            ////Dictionary<string, List<TimeIntervalSymbol>> tiListDic = twoSizedTIRPsMatrix[toncepts[tTrgtID].tonceptINDEX][toncepts[tErlyID].tonceptINDEX].prsMxRelVec[rel].instancesDicList; //entitiesKarmaVec[eIdx].instancesDic[trK];
            //Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] tiListDicsVec = karma.Kindex[toncepts[tTrgtID].tonceptINDEX, toncepts[tErlyID].tonceptINDEX].getRelEntitiesDics(rel);
            
            //Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] tiListDicsVec = getTindexRelEntitiesDicsVec(tTrgtIdx, tErlyIdx, rel);
            Dictionary<int, Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>> tiListDicsVec = getTindexRelEntitiesDicsVec(tTrgtIdx, tErlyIdx, rel);
            for (int eIdx = 0; eIdx < tiListDicsVec.Count(); eIdx++)
            {
                //Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTiDic = tiListDicsVec[eIdx];
                //if (tiListDicsVec[eIdx] != null)
                {
                    foreach (KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListDicsVec.ElementAt(eIdx).Value) // [eIdx]) // tiListTiDic)
                        foreach (TimeIntervalSymbol tis in tiListTi.Value)
                        {
                            TIsInstance tisInsNew = new TIsInstance(new TimeIntervalSymbol(tiListTi.Key.startTime, tiListTi.Key.endTime, tiListTi.Key.symbol), tis, tiListDicsVec.ElementAt(eIdx).Key); // eIdx);
                            twoSzdTIRP.addEntity(eIdx); //// int.Parse(tiListTi.Key.Split('-')[0]));
                            twoSzdTIRP.tinstancesList.Add(tisInsNew);
                            twoSzdTIRP.meanHorizontalSupport++;
                        }
                }
            }
            return twoSzdTIRP;
        }
    }

    public class Dharma
    {
        int dharmaIndexType; public void setDharmaIndexType(int setDharmaIndexType) { dharmaIndexType = setDharmaIndexType; }
        long mxAccsCounter;   public void   resetMxAccsCounter()    { mxAccsCounter = 0; } 
                              public long   getMxAccsCounter()      { return mxAccsCounter; }
        bool HS1;             public bool   getHS1()                { return HS1;   }
        int print;            public int    getPrint()              { return print; }
        bool trans;           public bool   getTrans()              { return trans; }    
                              public void   setTrans(bool seTrans)  { trans = seTrans; }
        int  relations_style; public int    getRelStyle()           { return relations_style; }  //number of reations (Allen7, KL3, KL2)
        int  epsilon;         public int    getEpsilon()            { return epsilon; }
        double min_ver_sup;   public double getMinVerSup()          { return min_ver_sup; }       //the exact value of the treshold in number of entities
        int  max_gap;         public int    getMaxGap()             { return max_gap; }
        int[][][] transition; public int    getFromRelation(int leftIdx, int topIdx) { return transition[leftIdx][topIdx][0]; }
                              public int    getToRelation(int leftIdx, int topIdx)   { return transition[leftIdx][topIdx][1]; }
        int entitieSize;      public int    getEntitieSize()        { return entitieSize; }
        int[] entitiesVec;    public int[]  getEntitiesVec()        { return entitiesVec; }
                              public int    getEntityByIdx(int idx) { return entitiesVec[idx]; }
                              int maxTirpSize; public int getMaxTirpSize() { return maxTirpSize; }
        public Dictionary<int, List<TimeIntervalSymbol>> entityTISs;
        Dictionary<int, TemporalConcept> toncepts = new Dictionary<int, TemporalConcept>();
                              public Dictionary<int, TemporalConcept>.ValueCollection getToncepts() { return toncepts.Values; }
                              public int getTonceptByIDVerticalSupport(int t_id) { return toncepts[t_id].getTonceptVerticalSupport(); } //public TemporalConcept getTonceptByID(int t_id) { return toncepts[t_id]; }
                              public int getTonceptIndexByID(int t_id) { if (toncepts.ContainsKey(t_id)) return toncepts[t_id].tonceptINDEX; else return -1; }
                              public TemporalConcept getTonceptByOrder(int idx) { return toncepts.Values.ElementAt(idx); }
                              public TemporalConcept getTonceptByID(int t_id) { if (containsTonceptID(t_id)) return toncepts[t_id]; else return null; }
                                
                              public bool containsTonceptID(int t_id) { return toncepts.ContainsKey(t_id); }
                              //public int getTonceptByIdHorizontalSupport(int t_id) { if (containsTonceptID(t_id)) return toncepts[t_id].getTonceptHorizontalDic().Count(); else return 0; }
        int forBackWards; public int getForBackWardsMining() { return forBackWards; }
        // the trick here is that the order of the toncepts is opposite. the indices of the relations are the same, but they are ordered in a different way
        //forward mining |0|1|3|6|      backwards mininig |9|8|7|6|
        //                 |2|4|7|                          |5|4|3|
        //                   |5|8|                            |2|1|
        //                     |9|                              |0|
        // the trick is with the priniting, for which we have int[][]    logiRelsIndxs;
        int[][]    logiRelsIndxs; public int[][] getBackwardsRelsIndxs() { return logiRelsIndxs; }
        ////twoSizedTIRPsMATRIXEntry[][] twoSizedTIRPsMatrix;
        ////twoSizedTIRPsMATRIXEntry[,] twoSizedTIRPsMatrixWithPsik;
        private dharmaIndex dharmaIdx; //public tindex karma;

        public int get2SizedVerticalSupport(int tTrgtIdx, int tErlyIdx, int rel)
        {
            return dharmaIdx.getSymSymRelVerticalSupport(tTrgtIdx, tErlyIdx, rel);
        }

        public List<TimeIntervalSymbol> get2SizedEntSTIListOfSTIs(int frstTndx, int scndTndx, int rel, int eIdx, TimeIntervalSymbol tis)
        {
            return dharmaIdx.getTindexRelEidxTisList(frstTndx, scndTndx, rel, eIdx, tis);
        }

        public TIRP get2SizedAsTIRP(int tTrgtID, int tErlyID, int rel)
        {
            return dharmaIdx.get2SizedAsTIRP(tTrgtID, tErlyID, toncepts[tTrgtID].tonceptINDEX, toncepts[tErlyID].tonceptINDEX, rel);
            /*TIRP twoSzdTIRP = new TIRP(tTrgtID, tErlyID, rel);
            //string trK = toncepts[tTrgtID].tonceptINDEX.ToString() + "-" + rel + "-" + toncepts[tErlyID].tonceptINDEX;
            //Dictionary<string, List<TimeIntervalSymbol>> tiListDic = twoSizedTIRPsMatrix[toncepts[tTrgtID].tonceptINDEX][toncepts[tErlyID].tonceptINDEX].prsMxRelVec[rel].instancesDicList; //entitiesKarmaVec[eIdx].instancesDic[trK];
            Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>>[] tiListDicsVec = karma.Kindex[toncepts[tTrgtID].tonceptINDEX, toncepts[tErlyID].tonceptINDEX].getRelEntitiesDics(rel);
            for(int eIdx = 0; eIdx < tiListDicsVec.Count(); eIdx++)
            {
                Dictionary<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTiDic = tiListDicsVec[eIdx];     
                foreach (KeyValuePair<TimeIntervalSymbol, List<TimeIntervalSymbol>> tiListTi in tiListTiDic)
                    foreach (TimeIntervalSymbol tis in tiListTi.Value)
                    {
                        TIsInstance tisInsNew = new TIsInstance(new TimeIntervalSymbol(tiListTi.Key.startTime, tiListTi.Key.endTime, tiListTi.Key.symbol), tis, eIdx);
                        twoSzdTIRP.addEntity(eIdx); // int.Parse(tiListTi.Key.Split('-')[0]));
                        twoSzdTIRP.tinstancesList.Add(tisInsNew);
                        twoSzdTIRP.meanHorizontalSupport++;
                    }
            }
            return twoSzdTIRP;
             */
        }
                
        public Dharma( int setMaxTirpSize, bool setFulleDics, int dharmaIndexType, bool runKarma, int setForBackWards, int setRelationStyle, int setEpsilon, int setMaxGap, int setMinVerSup, string dsFilePath, bool seTrans, bool setHS1, int setPrint, ref string runTime, int entitieSizeLimit = KLC.NUM_OF_ENTITIES)
        {
            DateTime starTime = DateTime.Now;
            string timings = "";
            mxAccsCounter = 0;
            forBackWards = setForBackWards;
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            HS1 = setHS1;
            maxTirpSize = setMaxTirpSize;   

            KarmE.read_tids_file(0, entitieSizeLimit, dsFilePath, ref entityTISs, ref toncepts);
            
            entitieSize = entityTISs.Count;
            min_ver_sup = ((double)setMinVerSup / 100) * entitieSize;
            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = entityTISs.Keys.ElementAt(eIdx);

            //twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][]; //initialize keys vector
            //for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
            //    twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS]; // initialize val vector
            
            //twoSizedTIRPsMatrixWithPsik = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS, KLC.NUM_OF_SYMBOLS];

            //karma = new tindex(entitieSize, relations_style);
            dharmaIdx = new dharmaIndex(dharmaIndexType, entitieSize, relations_style); //, setFulleDics);

            print = setPrint;
            trans = seTrans;
            if (trans)
            {
                string tesTrans;
                if (relations_style == KLC.RELSTYLE_ALLEN7)
                    tesTrans = KLC.LoadTransitionTableALLEN7(ref transition);//load transition table
                else
                    tesTrans = KLC.LoadTransitionTableKL3(ref transition);
                if (tesTrans.Length > 0)
                    transition = null;
            }

            if (forBackWards == KLC.backwardsMining)
            {
                logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
                logiRelsIndxs[0/*3*/] = new int[1]  { 0 }; 
                logiRelsIndxs[1/*3*/] = new int[3]  { 2, 1, 0 };
                logiRelsIndxs[2/*4*/] = new int[6]  { 5, 4, 2, 3, 1, 0 };
                logiRelsIndxs[3/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
                logiRelsIndxs[4/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
                logiRelsIndxs[5/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[6/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[7/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[8/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
            }

            if( runKarma == true )
                RunKarmaRun(null, 0, entitieSizeLimit);
           
            DateTime endTime = DateTime.Now;
            runTime = endTime.Subtract(starTime).TotalMilliseconds.ToString(); // ((endTime - starTime).Minutes * 60 + (endTime - starTime).Seconds) + "," + (endTime - starTime).Minutes + ":" + (endTime - starTime).Seconds;
        }

        // for Single KarmaLego
        public Dharma( int setMaxTirpSize, bool setFullDics, int dharmaIndexType, int frstEntIdx, int lstEntIdx, int setForBackWards, int setRelationStyle, int setEpsilon, int setMaxGap, string dsFilePath, bool runKarma, List<int> tonceptIds)
        {
            mxAccsCounter = 0;
            forBackWards = setForBackWards;
            relations_style = setRelationStyle;
            epsilon = setEpsilon;
            max_gap = setMaxGap;
            HS1 = false;
            maxTirpSize = setMaxTirpSize;

            KarmE.read_tids_file(frstEntIdx, lstEntIdx, dsFilePath, ref entityTISs, ref toncepts);

            entitieSize = entityTISs.Count;
            min_ver_sup = 0; // ((double)setMinVerSup / 100) * entitieSize;
            entitiesVec = new int[entitieSize];
            for (int eIdx = 0; eIdx < entitieSize; eIdx++)
                entitiesVec[eIdx] = entityTISs.Keys.ElementAt(eIdx);

            ////twoSizedTIRPsMatrix = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS][]; //initialize keys vector
            ////for (int i = 0; i < KLC.NUM_OF_SYMBOLS; i++)
            ////    twoSizedTIRPsMatrix[i] = new twoSizedTIRPsMATRIXEntry[KLC.NUM_OF_SYMBOLS]; // initialize val vector
            //karma = new tindex(entitieSize, relations_style);
            dharmaIdx = new dharmaIndex(dharmaIndexType, entitieSize, relations_style);//, setFullDics);
            //print = setPrint;
            //trans = seTrans;
            if (trans)
            {
                string tesTrans;
                if (relations_style == KLC.RELSTYLE_ALLEN7)
                    tesTrans = KLC.LoadTransitionTableALLEN7(ref transition);//load transition table
                else
                    tesTrans = KLC.LoadTransitionTableKL3(ref transition);
                if (tesTrans.Length > 0)
                    transition = null;
            }

            if (forBackWards == KLC.backwardsMining)
            {
                logiRelsIndxs = new int[KLC.MAX_TIRP_TONCEPTS_SIZE][];
                logiRelsIndxs[0/*3*/] = new int[1] { 0 };
                logiRelsIndxs[1/*3*/] = new int[3] { 2, 1, 0 };
                logiRelsIndxs[2/*4*/] = new int[6] { 5, 4, 2, 3, 1, 0 };
                logiRelsIndxs[3/*5*/] = new int[10] { 9, 8, 5, 7, 4, 2, 6, 3, 1, 0 };
                logiRelsIndxs[4/*6*/] = new int[15] { 14, 13, 9, 12, 8, 5, 11, 7, 4, 2, 10, 6, 3, 1, 0 };
                logiRelsIndxs[5/*7*/] = new int[21] { 20, 19, 14, 18, 13, 9, 17, 12, 8, 5, 16, 11, 7, 4, 2, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[6/*8*/] = new int[28] { 27, 26, 20, 25, 19, 14, 24, 18, 13, 9, 23, 17, 12, 8, 5, 22, 16, 11, 7, 4, 2, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[7/*9*/] = new int[36] { 35, 34, 27, 33, 26, 20, 32, 25, 19, 14, 31, 24, 18, 13, 9, 30, 23, 17, 12, 8, 5, 29, 22, 16, 11, 7, 4, 2, 28, 21, 15, 10, 6, 3, 1, 0 };
                logiRelsIndxs[8/*9*/] = new int[45] { 44, 43, 35, 42, 34, 27, 41, 33, 26, 20, 40, 32, 25, 19, 14, 39, 31, 24, 18, 13, 9, 38, 30, 23, 17, 12, 8, 5, 37, 29, 22, 16, 11, 7, 4, 2, 36, 28, 21, 15, 10, 6, 3, 1, 0 };
            }

            if (runKarma == true)
                RunKarmaRun(tonceptIds, frstEntIdx, lstEntIdx);

        }

        public void RunKarmaRun(List<int> tonceptIds, int from_eIdx, int to_eIdx)
        {
            int index = 0;
            int[] relStat = new int[7];
            for (int eIdx = from_eIdx; eIdx < to_eIdx &&  eIdx < entityTISs.Count() && eIdx < entitieSize; eIdx++)
                indexEntitySTIs(eIdx, ref index, ref relStat, tonceptIds);
        }

        public void indexEntitySTIs(int eIdx, ref int index, ref int[] relStat, List<int> tonceptIds)
        {
            List<TimeIntervalSymbol> tisList = entityTISs[entitiesVec[eIdx]];

            for (int ti1Idx = 0; ti1Idx < tisList.Count; ti1Idx++)
            {
                TimeIntervalSymbol firsTis = tisList.ElementAt(ti1Idx);
                if (tonceptIds == null || tonceptIds.Contains(firsTis.symbol))
                {
                    addToncept(eIdx, firsTis, ref index); // update toncept entity support
                    //karma..
                    for (int ti2Idx = ti1Idx + 1; ti2Idx < tisList.Count; ti2Idx++)
                    {
                        TimeIntervalSymbol secondTis = tisList.ElementAt(ti2Idx);
                        if (tonceptIds == null || tonceptIds.Contains(secondTis.symbol))
                        {
                            if (firsTis.symbol != secondTis.symbol)
                            {
                                addToncept(eIdx, secondTis, ref index); // DONT update toncept entity support
                                int relation = KLC.WhichRelationEpsilon(firsTis, secondTis, relations_style, epsilon, max_gap);
                                if (relation > -1)
                                {
                                    relStat[relation]++;
                                    indexTISPair(firsTis, secondTis, relation, eIdx);
                                }
                                else
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private void indexTISPair(TimeIntervalSymbol firsTis, TimeIntervalSymbol secondTis, int relation, int eIdx)
        {
            if (forBackWards == KLC.forwardMining)
                indexTimeInetervalSymbolsPair(firsTis, secondTis, relation, eIdx);
            else
                indexTimeInetervalSymbolsPair(secondTis, firsTis, relation, eIdx);
        }

        private void addToncept(int entityIdx, TimeIntervalSymbol tis, ref int index)
        {
            TemporalConcept tc;
            if (!toncepts.ContainsKey(tis.symbol))
            {
                tc = new TemporalConcept(tis.symbol, toncepts.Count());// index);
                index++;
                toncepts.Add(tis.symbol, tc);
            }
            else
                tc = toncepts[tis.symbol];
            tc.addEntityTinstance(entityIdx, tis); // tc.addEntity(entityIdx);
        }

        private void indexTimeInetervalSymbolsPair(TimeIntervalSymbol tisKey, TimeIntervalSymbol tisVal, int relation, int entityIdx)
        {
            int tKeyIdx = toncepts[tisKey.symbol].tonceptINDEX;
            int tValIdx = toncepts[tisVal.symbol].tonceptINDEX;
            //karma.indexTTsRelEidxSTIs(tKeyIdx, tValIdx, relation, entityIdx, tisKey, tisVal);
            dharmaIdx.indexTTsRelEidxSTIs(tKeyIdx, tValIdx, relation, entityIdx, tisKey, tisVal, toncepts.Count());
        }
    }
}
