export class ShopParams {
  public static PageNumberDefault = 1;
  public static PageSizeDefault = 6;

  brandId: string = '';
  typeId: string = '';
  sort = 'name';
  pageNumber = ShopParams.PageNumberDefault;
  pageSize = ShopParams.PageSizeDefault;
  search: string = '';
}
