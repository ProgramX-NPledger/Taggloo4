import { Language } from "./language";

export interface LanguagesResult {
  results: Language[];
  fromIndex: number;
  totalItemsCount: number;
  pageSize: number;
}
