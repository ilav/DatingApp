import { Photo } from './photo';

export interface User {
  id: number;
  userName: string;
  knownAs: string;
  age: number;
  gender: string;
  created: Date;
  lastActive: any;
  photoUrl: string;
  city: string;
  country: string;

  // This is optional that is why we use the 'elvies' or '?' operator
  // Optional properties should come after the required properties or else you will get
  // an error
  interests?: string;
  introduction?: string;
  lookingFor?: string;
  photos?: Photo[];
  //roles?: string[];
}
