export class DescriptiveResponse<T>{
  IsErrorState: boolean;
  Value: any;
  ErrorDescription :string;
  Errors: string[];
  ErrorMetadata: string;
}
