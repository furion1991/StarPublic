import { API } from '@/shared/api'

import type { BestDropResponse, Item } from './loot.types'

export const getCaseItem = async (itemId: string) => {
  const { data } = await API.get<Item>(`/items/get/${itemId}`)

  return data
}

export const getBestDrop = async (userId: string) => {
  const { data } = await API.get<BestDropResponse>(`/audit/max_cost_item/${userId}`)

  return data.result
}
