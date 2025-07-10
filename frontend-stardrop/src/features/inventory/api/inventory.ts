import { API } from '@/shared/api'

export const sellInventoryLootItems = (inventoryItemsIds: string[]) => {
  return API.post('/inventory/sell-item', inventoryItemsIds)
}
