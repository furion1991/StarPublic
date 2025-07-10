import { API } from '@/shared/api'

import type { BestDropResponse, GetAllItemsProps, Loot } from './loot.types'

export const getCaseItem = async (itemId: string) => {
  const { data } = await API.get<Loot>(`/items/get/${itemId}`)

  return data
}

export const getBestDrop = async (userId: string) => {
  const { data } = await API.get<BestDropResponse>(`/audit/max_cost_item/${userId}`)

  return data.result
}

export const getAllItems = async ({ valueFrom, nameSearch }: GetAllItemsProps) => {
  const { data } = await API.get<Loot[]>('/items/getall', {
    params: {
      FilterBy: nameSearch,
      FilterValue: valueFrom,
      Count: 100
    }
  })

  return data
}
