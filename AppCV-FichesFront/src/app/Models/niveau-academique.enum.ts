export class NiveauAcademique {
  label: string;
  value: number;

  Collection: Array<NiveauAcademique> = new Array<NiveauAcademique> () ;

  constructor() {
     this.Collection =  [
      { label  :"Primaire", value : 0 },
      { label  :"Secondaire", value : 1 },
      { label  :"DEC", value : 3 },
      { label  :"BAC", value : 4 },
      { label  :"Maitre", value : 5 },
      { label  :"Doctorat", value : 6 },
      { label  :"Nule", value : 9 },
    ] as Array<NiveauAcademique>;
  }
}
