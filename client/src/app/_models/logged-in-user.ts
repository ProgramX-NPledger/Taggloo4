export interface LoggedInUser {
  UserName: string;
  Token: string;
  Email: string | null;
  EmailSha256: string | null;
}
