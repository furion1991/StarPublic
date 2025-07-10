import { useMutation, useQueryClient } from '@tanstack/react-query'

import { sellInventoryLootItems } from '../api/inventory'

type UseSellItemsProps = {
  onSuccess?: () => void
}

export const useSellItems = ({ onSuccess }: UseSellItemsProps = {}) => {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: sellInventoryLootItems,
    onSuccess: () => {
      // update inventory items
      queryClient.invalidateQueries({
        queryKey: ['me']
      })

      if (onSuccess) {
        onSuccess()
      }
    }
  })
}
