import { CertificationHighlight } from "./Highlight/Certification-HighLight";
import { EditionObjecViewModel } from "./EditionObjec-model";

export class CertificationViewModel {
  graphId: string;
  graphIdGenre: string;
  description?: string;
  annee?: number;
  certificationHighlight:CertificationHighlight = new CertificationHighlight();
  editionObjecViewModels: Array<EditionObjecViewModel>;
}

