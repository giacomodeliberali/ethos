import { HttpClient, HttpHeaders, HttpResponse } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

interface RequestOptionsArgs {
  body?: any;
  headers?: HttpHeaders;
  observe?: 'response';
  responseType?: 'blob' | 'json';
}

export class NswagBaseClass {
  constructor(private httpClient: HttpClient = null) {}

  protected processRequest(method: string, url: string, options: RequestOptionsArgs, isReturnTypeBinary: boolean): Observable<any> {
    const requestOptions: RequestOptionsArgs = {
      ...options,
      responseType: isReturnTypeBinary ? 'blob' : 'json', // se il backend ritorna binary, stiamo scaricando un file
    };

    return this.httpClient.request(method, url, requestOptions).pipe(
      mergeMap((httpResponse: HttpResponse<any>) => {
        if (httpResponse.ok) {
          if (isReturnTypeBinary) {
            // con questo possiamo salvare il file con this.salvaFile(httpResponse)
            return of(httpResponse);
          }
          return of(httpResponse.body);
        }

        // lanciamo l'errore che viene intercettato da AbpHttpInterceptor per mostrare il toast
        return throwError(httpResponse);
      })
    );
  }
}
