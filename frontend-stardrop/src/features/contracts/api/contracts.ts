import { API } from '@/shared/api'

import type {
  CreateContractResponse,
  ContractItemApproximateValueResponse,
  CreateContractProps
} from './contracts.types'

export const getContractItemApproximateValue = async (itemsIds: string[]) => {
  const { data } = await API.post<ContractItemApproximateValueResponse>('/contracts/preview', {
    itemsList: itemsIds
  })

  return data.result
}

export const createContract = async ({ inventoryItemsIds, userId }: CreateContractProps) => {
  const { data } = await API.post<CreateContractResponse>('/contracts/execute', {
    itemRecordIds: inventoryItemsIds,
    userId
  })

  return data.result.items[0]
}
