import Link from 'next/link'

import { Button, NoDataPanel } from '@/shared/ui'
import { ContractHistoryItem } from '@/entities/contracts'

import classes from './ContractsHistoryList.module.scss'

export const ContractsHistoryList = () => {
  const contracts = Array.from({ length: 4 })
    .fill(null)
    .map((_, idx) => <ContractHistoryItem key={idx} />)

  if (!contracts.length) {
    return (
      <NoDataPanel
        title='Нет контрактов'
        text='Создайте свой первый контракт'
        action={
          <Link href='/'>
            <Button>Контракты ›</Button>
          </Link>
        }
      />
    )
  }

  return <div className={classes.contractsHistoryList}>{contracts}</div>
}
