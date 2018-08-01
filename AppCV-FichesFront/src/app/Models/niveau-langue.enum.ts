export class NiveauLangue {
  label: string;
  value: string;

  Collection: Array<NiveauLangue> = new Array<NiveauLangue> () ;

  constructor() {
     this.Collection =  [
      { label  :"Basique", value : "Basique" },
      { label  :"Intermédiaire", value : "Intermédiaire" },
      { label  :"Avancé", value : "Avancé" },

    ] as Array<NiveauLangue>;
  }
}
