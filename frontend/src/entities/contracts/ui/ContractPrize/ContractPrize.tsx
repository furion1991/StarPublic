import Image from 'next/image'

import classes from './ContractPrize.module.scss'

type ContractPrizeProps = {
  prizeImg: string
}

export const ContractPrize = ({ prizeImg }: ContractPrizeProps) => {
  return (
    <div className={classes.contractPrize}>
      <Image className={classes.prizeImg} src={prizeImg} width={253} height={253} alt='Приз' />

      <Image src='/img/contract-prize-bg.png' width={778} height={489} alt='Фон' />
    </div>
  )
}
